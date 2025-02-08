using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using SharpDX;
using SharpDX.Direct3D12;
using SharpDX.DXGI;
using Resource = SharpDX.Direct3D12.Resource;

/*
you have to install SharpDX first. use NuGet in Visual Studio :

Install-Package SharpDX
Install-Package SharpDX.Direct2D1
Install-Package SharpDX.DirectWrite
 */


// to display TEXT on screen
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.Mathematics.Interop;



namespace DX12GameProgramming
{
    public class MathFunc3DListApp : D3DApp
    {
        private readonly List<FrameResource> _frameResources = new List<FrameResource>(NumFrameResources);
        private readonly List<AutoResetEvent> _fenceEvents = new List<AutoResetEvent>(NumFrameResources);

        private int _currFrameResourceIndex;
        private int _gridFrameResourceIndex;

        private RootSignature _rootSignature;

        private readonly Dictionary<string, MeshGeometry> _geometries = new Dictionary<string, MeshGeometry>();
        private readonly Dictionary<string, ShaderBytecode> _shaders = new Dictionary<string, ShaderBytecode>();
        private readonly Dictionary<string, PipelineState> _psos = new Dictionary<string, PipelineState>();

        private InputLayoutDescription _inputLayout;

        // List of all the render items.
        private readonly List<RenderItem> _allRitems = new List<RenderItem>();

        // Render items divided by PSO.
        private readonly Dictionary<RenderLayer, List<RenderItem>> _ritemLayers = new Dictionary<RenderLayer, List<RenderItem>>(1)
        {
            [RenderLayer.Opaque] = new List<RenderItem>()
        };

      
        private PassConstants _mainPassCB;

        private bool _isWireframe ;

        private Vector3 _eyePos;
        private Matrix _proj = Matrix.Identity;
        private Matrix _view = Matrix.Identity;

        private float _theta = 1.5f * MathUtil.Pi;
        private float _phi = MathUtil.PiOverTwo - 0.4f;
        private float _radius = 250.0f;

        private Point _lastMousePos;


        /// <summary>
        /// Text on Screen
        /// </summary>
        private WindowRenderTarget renderTarget;
        private SolidColorBrush brush;
        private TextFormat textFormat;

        public MathFunc3DListApp()
        {
            MainWindowCaption = "list of 3D math funcs";
        }

        private FrameResource CurrFrameResource => _frameResources[_currFrameResourceIndex];
        private AutoResetEvent CurrentFenceEvent => _fenceEvents[_currFrameResourceIndex];

        public override void Initialize()
        {
            base.Initialize();

            InitializeResources();

            /*
            // Initialize renderTarget here
            var renderTargetProperties = new RenderTargetProperties();
            var hwndRenderTargetProperties = new HwndRenderTargetProperties
            {
                //Hwnd = d3d.MainWindow.Handle,

                PixelSize = new Size2(ClientWidth, ClientHeight),
                PresentOptions = PresentOptions.None
            };

            renderTarget = new WindowRenderTarget(new SharpDX.Direct2D1.Factory(), renderTargetProperties, hwndRenderTargetProperties);
            */

            // Reset the command list to prep for initialization commands.
            CommandList.Reset(DirectCmdListAlloc, null);

            BuildRootSignature();
            BuildShadersAndInputLayout();


            ////////////
            BuildGauss3DGraph();
            BuildRotSym3DGraph();

            BuildRenderItems();

            BuildFrameResources();
            BuildPSOs();

            // Execute the initialization commands.
            CommandList.Close();

            // Add the commands to the queue for execution.
            CommandQueue.ExecuteCommandList(CommandList);

            // Wait until initialization is complete.
            FlushCommandQueue();
        }

        protected override void OnResize()
        {
            base.OnResize();

            // The window resized, so update the aspect ratio and recompute the projection matrix.
            _proj = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, AspectRatio, 1.0f, 1000.0f);
        }

        protected override void Update(GameTimer gt)
        {
            UpdateCamera(gt);

            // Cycle through the circular frame resource array.
            _currFrameResourceIndex = (_currFrameResourceIndex + 1) % NumFrameResources;

            // Has the GPU finished processing the commands of the current frame resource?
            // If not, wait until the GPU has completed commands up to this fence point.
            if (CurrFrameResource.Fence != 0 && Fence.CompletedValue < CurrFrameResource.Fence)
            {
                Fence.SetEventOnCompletion(CurrFrameResource.Fence, CurrentFenceEvent.SafeWaitHandle.DangerousGetHandle());
                CurrentFenceEvent.WaitOne();
            }

            UpdateObjectCBs();
            UpdateMainPassCB(gt);

        }

        protected override void Draw(GameTimer gt)
        {
            CommandAllocator cmdListAlloc = CurrFrameResource.CmdListAlloc;

            // Reuse the memory associated with command recording.
            // We can only reset when the associated command lists have finished execution on the GPU.
            cmdListAlloc.Reset();


          


            // A command list can be reset after it has been added to the command queue via ExecuteCommandList.
            // Reusing the command list reuses memory.

            CommandList.Reset(cmdListAlloc, ! _isWireframe ? _psos["opaque_wireframe"] : _psos["opaque"]);

            CommandList.SetViewport(Viewport);
            CommandList.SetScissorRectangles(ScissorRectangle);

            // Indicate a state transition on the resource usage.
            CommandList.ResourceBarrierTransition(CurrentBackBuffer, ResourceStates.Present, ResourceStates.RenderTarget);

            // Clear the back buffer and depth buffer.
            CommandList.ClearRenderTargetView(CurrentBackBufferView, Color.Blue);
            CommandList.ClearDepthStencilView(DepthStencilView, ClearFlags.FlagsDepth | ClearFlags.FlagsStencil, 1.0f, 0);

            // Specify the buffers we are going to render to.
            CommandList.SetRenderTargets(CurrentBackBufferView, DepthStencilView);

            CommandList.SetGraphicsRootSignature(_rootSignature);

            // Bind per-pass constant buffer. We only need to do this once per-pass.
            Resource passCB = CurrFrameResource.PassCB.Resource;
            CommandList.SetGraphicsRootConstantBufferView(1, passCB.GPUVirtualAddress);




            DrawRenderItems(CommandList, _ritemLayers[RenderLayer.Opaque]);
            // DrawText("Gaussian Model 3D", 10, 10);





            // Indicate a state transition on the resource usage.
            CommandList.ResourceBarrierTransition(CurrentBackBuffer, ResourceStates.RenderTarget, ResourceStates.Present);

            // Done recording commands.
            CommandList.Close();

            // Add the command list to the queue for execution.
            CommandQueue.ExecuteCommandList(CommandList);

            // Present the buffer to the screen. Presenting will automatically swap the back and front buffers.
            SwapChain.Present(0, PresentFlags.None);

            // Advance the fence value to mark commands up to this fence point.
            CurrFrameResource.Fence = ++CurrentFence;

            // Add an instruction to the command queue to set a new fence point.
            // Because we are on the GPU timeline, the new fence point won't be
            // set until the GPU finishes processing all the commands prior to this Signal().
            CommandQueue.Signal(Fence, CurrentFence);
        }

        protected override void OnMouseDown(MouseButtons button, Point location)
        {
            base.OnMouseDown(button, location);
            _lastMousePos = location;
        }

        protected override void OnMouseMove(MouseButtons button, Point location)
        {
            if ((button & MouseButtons.Left) != 0)
            {
                // Make each pixel correspond to a quarter of a degree.
                float dx = MathUtil.DegreesToRadians(0.25f * (location.X - _lastMousePos.X));
                float dy = MathUtil.DegreesToRadians(0.25f * (location.Y - _lastMousePos.Y));

                // Update angles based on input to orbit camera around box.
                _theta += dx;
                _phi += dy;

                // Restrict the angle mPhi.
                _phi = MathUtil.Clamp(_phi, 0.1f, MathUtil.Pi - 0.1f);
            }
            else if ((button & MouseButtons.Right) != 0)
            {
                // Make each pixel correspond to a quarter of a degree.
                float dx = 0.2f * (location.X - _lastMousePos.X);
                float dy = 0.2f * (location.Y - _lastMousePos.Y);

                // Update the camera radius based on input.
                _radius += dx - dy;

                // Restrict the radius.
                _radius = MathUtil.Clamp(_radius, 5.0f, 150.0f);
            }

            _lastMousePos = location;
        }

        protected override void OnKeyDown(Keys keyCode)
        {
            if (keyCode == Keys.D1)
                _isWireframe = true;
        }

        protected override void OnKeyUp(Keys keyCode)
        {
            base.OnKeyUp(keyCode);

            if (keyCode == Keys.D1)
                _isWireframe = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _rootSignature?.Dispose();

                foreach (FrameResource frameResource in _frameResources) frameResource.Dispose();
                foreach (MeshGeometry geometry in _geometries.Values) geometry.Dispose();
                foreach (PipelineState pso in _psos.Values) pso.Dispose();
            }
            base.Dispose(disposing);
        }

        private void UpdateCamera(GameTimer gt)
        {
            // Convert Spherical to Cartesian coordinates.
            _eyePos.X = _radius * MathHelper.Sinf(_phi) * MathHelper.Cosf(_theta * gt.TotalTime / 100.0f);
            _eyePos.Z = _radius * MathHelper.Sinf(_phi) * MathHelper.Sinf(_theta * gt.TotalTime / 100.0f);

            _eyePos.Y = _radius * MathHelper.Cosf(_phi);

            // Build the view matrix.
            _view = Matrix.LookAtLH(_eyePos, Vector3.Zero, Vector3.Up);
        }

        private void UpdateObjectCBs()
        {
            foreach (RenderItem e in _allRitems)
            {
                // Only update the cbuffer data if the constants have changed.
                // This needs to be tracked per frame resource.
                if (e.NumFramesDirty > 0)
                {
                    var objConstants = new ObjectConstants { World = Matrix.Transpose(e.World) };
                    CurrFrameResource.ObjectCB.CopyData(e.ObjCBIndex, ref objConstants);

                    // Next FrameResource need to be updated too.
                    e.NumFramesDirty--;
                }
            }
        }

        private void UpdateMainPassCB(GameTimer gt)
        {
            Matrix viewProj = _view * _proj;
            Matrix invView = Matrix.Invert(_view);
            Matrix invProj = Matrix.Invert(_proj);
            Matrix invViewProj = Matrix.Invert(viewProj);

            _mainPassCB.View = Matrix.Transpose(_view);
            _mainPassCB.InvView = Matrix.Transpose(invView);
            _mainPassCB.Proj = Matrix.Transpose(_proj);
            _mainPassCB.InvProj = Matrix.Transpose(invProj);
            _mainPassCB.ViewProj = Matrix.Transpose(viewProj);
            _mainPassCB.InvViewProj = Matrix.Transpose(invViewProj);
            _mainPassCB.EyePosW = _eyePos;
            _mainPassCB.RenderTargetSize = new Vector2(ClientWidth, ClientHeight);
            _mainPassCB.InvRenderTargetSize = 1.0f / _mainPassCB.RenderTargetSize;
            _mainPassCB.NearZ = 1.0f;
            _mainPassCB.FarZ = 1000.0f;
            _mainPassCB.TotalTime = gt.TotalTime;
            _mainPassCB.DeltaTime = gt.DeltaTime;

            CurrFrameResource.PassCB.CopyData(0, ref _mainPassCB);
        }

        private void BuildRootSignature()
        {
            // Root parameter can be a table, root descriptor or root constants.
            var slotRootParameters = new[]
            {
                // TODO: API suggestion: RootDescriptor register space default value = 0.
                new RootParameter(ShaderVisibility.Vertex, new RootDescriptor(0, 0), RootParameterType.ConstantBufferView),
                new RootParameter(ShaderVisibility.Vertex, new RootDescriptor(1, 0), RootParameterType.ConstantBufferView)
            };

            // A root signature is an array of root parameters.
            var rootSigDesc = new RootSignatureDescription(
                RootSignatureFlags.AllowInputAssemblerInputLayout,
                slotRootParameters);

            // Create a root signature with a single slot which points to a descriptor range consisting of a single constant buffer.
            _rootSignature = Device.CreateRootSignature(rootSigDesc.Serialize());
        }

        private void BuildShadersAndInputLayout()
        {
            _shaders["standardVS"] = D3DUtil.CompileShader("Shaders\\Color.hlsl", "VS", "vs_5_0");
            _shaders["opaquePS"] = D3DUtil.CompileShader("Shaders\\Color.hlsl", "PS", "ps_5_0");

            _inputLayout = new InputLayoutDescription(new[]
            {
                new SharpDX.Direct3D12.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                new SharpDX.Direct3D12.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0)
            });
        }

        private void BuildGauss3DGraph()
        {
            //CreateGrid(width,length, x num of vertex elements, z num of vertex elements)   
            GeometryGenerator.MeshData grid = GeometryGenerator.CreateGrid(140.0f, 140.0f, 50, 50);
                                               

            //
            // Extract the vertex elements we are interested and apply the height function to
            // each vertex. In addition, color the vertices based on their height
            //

            _gridFrameResourceIndex = grid.Vertices.Count;

            var vertices = new Vertex[_gridFrameResourceIndex];

            for (int i = 0; i < _gridFrameResourceIndex; i++)
            {
                Vector3 p = grid.Vertices[i].Position;

                vertices[i].Pos   = p;
                vertices[i].Pos.Y =  (float) GaussFuncValue(p.X, p.Z);

                // Color the vertex based on its height.
                if (vertices[i].Pos.Y < 0.0f)
                {
                    // Sandy beach color.
                    vertices[i].Color = new Vector4(1.0f, 0.96f, 0.62f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 10.0f)
                {
                    // Light yellow-green.
                    vertices[i].Color = new Vector4(0.48f, 0.77f, 0.46f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 35.0f)
                {
                    // Dark yellow-green.
                    vertices[i].Color = new Vector4(0.1f, 0.48f, 0.19f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 65.0f)
                {
                    // Dark brown.
                    vertices[i].Color = new Vector4(0.45f, 0.39f, 0.34f, 1.0f);
                }
                else
                {
                    // White snow.
                    vertices[i].Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }

            List<short> indices = grid.GetIndices16();

            var geo = MeshGeometry.New(Device, CommandList, vertices, indices.ToArray(), "landGeo");

            var submesh = new SubmeshGeometry
            {
                IndexCount = indices.Count,
                StartIndexLocation = 0,
                BaseVertexLocation = 0
            };

            geo.DrawArgs["gauss"] = submesh;

            _geometries["landGeo"] = geo;
        }

        private void BuildRotSym3DGraph()
        {
            GeometryGenerator.MeshData grid = GeometryGenerator.CreateGrid(160.0f, 160.0f, 50, 50);

            //
            // Extract the vertex elements we are interested and apply the height function to
            // each vertex. In addition, color the vertices based on their height so we have
            // sandy looking beaches, grassy low hills, and snow mountain peaks.
            //

            _gridFrameResourceIndex = grid.Vertices.Count;

            //var vertices = new Vertex[grid.Vertices.Count];
            var vertices = new Vertex[_gridFrameResourceIndex];

            for (int i = 0; i < _gridFrameResourceIndex; i++)
            {
                Vector3 p = grid.Vertices[i].Position;
                vertices[i].Pos = p;
                vertices[i].Pos.Y = (float)RotSymFuncValue(p.X, p.Z);

                // Color the vertex based on its height.
                if (vertices[i].Pos.Y < -10.0f)
                {
                    // Sandy beach color.
                    vertices[i].Color = new Vector4(1.0f, 0.96f, 0.62f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 5.0f)
                {
                    // Light yellow-green.
                    vertices[i].Color = new Vector4(0.48f, 0.77f, 0.46f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 12.0f)
                {
                    // Dark yellow-green.
                    vertices[i].Color = new Vector4(0.1f, 0.48f, 0.19f, 1.0f);
                }
                else if (vertices[i].Pos.Y < 20.0f)
                {
                    // Dark brown.
                    vertices[i].Color = new Vector4(0.45f, 0.39f, 0.34f, 1.0f);
                }
                else
                {
                    // White snow.
                    vertices[i].Color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
                }
            }

            List<short> indices = grid.GetIndices16();

            var geo = MeshGeometry.New(Device, CommandList, vertices, indices.ToArray(), "landGeo");

            var submesh = new SubmeshGeometry
            {
                IndexCount = indices.Count,
                StartIndexLocation = 0,
                BaseVertexLocation = 0
            };

            geo.DrawArgs["rotsym"] = submesh;

            _geometries["landGeo"] = geo;
        }

      

        private void BuildPSOs()
        {
            //
            // PSO for opaque objects.
            //

            var opaquePsoDesc = new GraphicsPipelineStateDescription
            {
                InputLayout = _inputLayout,
                RootSignature = _rootSignature,
                VertexShader = _shaders["standardVS"],
                PixelShader = _shaders["opaquePS"],
                RasterizerState = RasterizerStateDescription.Default(),
                BlendState = BlendStateDescription.Default(),
                DepthStencilState = DepthStencilStateDescription.Default(),
                SampleMask = int.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                SampleDescription = new SampleDescription(MsaaCount, MsaaQuality),
                DepthStencilFormat = DepthStencilFormat
            };

            opaquePsoDesc.RenderTargetFormats[0] = BackBufferFormat;

            _psos["opaque"] = Device.CreateGraphicsPipelineState(opaquePsoDesc);

            //
            // PSO for opaque wireframe objects.
            //

            var opaqueWireframePsoDesc = opaquePsoDesc;

            opaqueWireframePsoDesc.RasterizerState.FillMode = SharpDX.Direct3D12.FillMode.Wireframe;

            _psos["opaque_wireframe"] = Device.CreateGraphicsPipelineState(opaqueWireframePsoDesc);
        }

        private void BuildFrameResources()
        {
            for (int i = 0; i < NumFrameResources; i++)
            {
                _frameResources.Add(new FrameResource(Device, 1, _allRitems.Count, _gridFrameResourceIndex));
                _fenceEvents.Add(new AutoResetEvent(false));
            }
        }

        private void BuildRenderItems()
        {
            //AddRenderItem(RenderLayer.Opaque, 0, "landGeo", "gauss");
            AddRenderItem(RenderLayer.Opaque, 0, "landGeo", "rotsym");

        }

        private RenderItem AddRenderItem(RenderLayer layer, int objCBIndex, string geoName, string submeshName)
        {
            MeshGeometry geo = _geometries[geoName];
            SubmeshGeometry submesh = geo.DrawArgs[submeshName];

            var renderItem = new RenderItem
            {
                ObjCBIndex = objCBIndex,
                Geo = geo,
                IndexCount = submesh.IndexCount,
                StartIndexLocation = submesh.StartIndexLocation,
                BaseVertexLocation = submesh.BaseVertexLocation
            };

            _ritemLayers[layer].Add(renderItem);
            _allRitems.Add(renderItem);

            return renderItem;
        }

        private void DrawRenderItems(GraphicsCommandList cmdList, List<RenderItem> ritems)
        {
            int objCBByteSize = D3DUtil.CalcConstantBufferByteSize<ObjectConstants>();
            Resource objectCB = CurrFrameResource.ObjectCB.Resource;

            foreach (RenderItem ri in ritems)
            {
                cmdList.SetVertexBuffer(0, ri.Geo.VertexBufferView);
                cmdList.SetIndexBuffer(ri.Geo.IndexBufferView);

                

                cmdList.PrimitiveTopology = ri.PrimitiveType;

                long objCBAddress = objectCB.GPUVirtualAddress + ri.ObjCBIndex * objCBByteSize;

                cmdList.SetGraphicsRootConstantBufferView(0, objCBAddress);

                cmdList.DrawIndexedInstanced(ri.IndexCount, 1, ri.StartIndexLocation, ri.BaseVertexLocation, 0);
            }
        }

        public void DrawText(string text, float x, float y)
        {
            renderTarget.BeginDraw();
            renderTarget.DrawText(text, textFormat, new RawRectangleF(x, y, x + 500, y + 50), brush);
            renderTarget.EndDraw();
        }


        private void InitializeResources()
        {

            var factory = new SharpDX.DirectWrite.Factory();

            textFormat = new TextFormat(factory, "Arial", FontWeight.Normal, FontStyle.Normal, 32);

            
            // 1. Create RTV Descriptor Heap
            var rtvHeapDesc = new DescriptorHeapDescription
            {
                DescriptorCount = 1,
                Type = DescriptorHeapType.RenderTargetView,
                Flags = DescriptorHeapFlags.None
            };
            var rtvHeap = Device.CreateDescriptorHeap(rtvHeapDesc);

            // 2. Create Render Target
            var renderTargetDesc = new ResourceDescription
            {
                Dimension = ResourceDimension.Texture2D,
                Alignment = 0,
                Width = 1280,
                Height = 720,
                DepthOrArraySize = 1,
                MipLevels = 1,
                Format = Format.R8G8B8A8_UNorm,
                SampleDescription = new SampleDescription(1, 0),
                Layout = TextureLayout.Unknown,
                Flags = ResourceFlags.AllowRenderTarget
            };

            var clearValue = new ClearValue
            {
                Format = Format.R8G8B8A8_UNorm,
                Color = new RawVector4(0, 0, 0, 1) // Changed from Color4 to RawVector4
            };

            
            //renderTarget = new RenderTarget();

            
              var renderTarget = Device.CreateCommittedResource(
                  new HeapProperties(HeapType.Default),
                  HeapFlags.None,
                  renderTargetDesc,
                  ResourceStates.Present,
                  clearValue
              );


              // 3. Create RTV
              var rtvHandle = rtvHeap.CPUDescriptorHandleForHeapStart;
              Device.CreateRenderTargetView(renderTarget, null, rtvHandle);


              /*
              // 4. Bind Render Target in Command List
              commandList.ResourceBarrierTransition(renderTarget, ResourceStates.Present, ResourceStates.RenderTarget);
              commandList.SetRenderTargets(rtvHandle, null);
              commandList.ClearRenderTargetView(rtvHandle, new Color4(0, 0, 0, 1), 0, null);

              // 5. After rendering
              commandList.ResourceBarrierTransition(renderTarget, ResourceStates.RenderTarget, ResourceStates.Present);
            

              brush = new SolidColorBrush(renderTarget, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
              */

        }

        private static float GaussFuncValue(float x, float z) => 80.0f * (float)Math.Exp(-(Math.Pow((x/25.0f), 2)+ Math.Pow((z / 25.0f), 2)));

        private static float RotSymFuncValue(float x, float z)
        {

            double r = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(z, 2));

            double result = (50.0f * (Math.Sin(r / 7.0f)) / (r / 7.0f));
            //double result = (50.0f * (Math.Cos(r / 7.0f)) / (r / 7.0f));

            return (float)result;

        }

    }


}
