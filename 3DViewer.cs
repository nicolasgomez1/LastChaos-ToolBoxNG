using SlimDX;
using SlimDX.Direct3D9;

namespace LastChaos_ToolBoxNG
{
	// SOURCE: Who knows.
	public partial class RenderDialog : Form, IDisposable
	{
		private static Main pMain;
		private static tMeshContainer? pMesh;
		private ASCIIEncoding pEnc = new();
		private Device? pDevice;
		private List<tMesh>? pModel;
		private float fZoom, fUpDown, fLeftRight, fRotation;
		private Vector3 vecCameraPosition = new(0.0f, -2.0f, 4f);
		private Vector3 vecEntityPosition = new(0f, 0.0f, 0.0f);
		private static string? strFilePath;
		private Texture? pMissingTexture;   // FIX out of range error
		private PresentParameters? pPresentParameters;
		private bool bDeviceReset = false;
		private bool bCaptureShot = false;

		public RenderDialog(Main mainForm)
		{
			InitializeComponent();

			pMain = mainForm;
		}

		private void RenderDialog_Load(object sender, EventArgs e)
		{
			timerRender.Start();

			panel3DView.MouseWheel += panel3DView_Zoom;

			InitializeDevice();
		}

		private void RenderDialog_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (pModel != null)
			{
				foreach (var model in pModel)
					model?.Dispose();

				pModel = null;

				timerRender.Stop();
				timerRender.Tick -= timerRender_Tick;
				timerRender.Dispose();
				timerRender = null;
			}

			pMissingTexture?.Dispose();
			pMissingTexture = null;

			pDevice?.Dispose();
			pDevice = null;
		}

		private void RenderDialog_Resize(object sender, EventArgs e)
		{
			if (pDevice == null || pPresentParameters == null)
				return;

			pPresentParameters.BackBufferWidth = panel3DView.Width;
			pPresentParameters.BackBufferHeight = panel3DView.Height;

			pDevice.Reset(pPresentParameters);

			pDevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(100f, (float)panel3DView.Width / panel3DView.Height, 1f, 100f));

			bDeviceReset = true;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if ((keyData & Keys.Alt) == Keys.Alt)
				return true;

			if ((keyData & Keys.C) == Keys.C && bCaptureShot == false)
			{
				bCaptureShot = true;
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void panel3DView_Zoom(object? sender, MouseEventArgs e)
		{
			if (ModifierKeys.HasFlag(Keys.Control))
			{
				fUpDown += Math.Sign(e.Delta) * 0.2f;
				fUpDown = Math.Max(-5, Math.Min(5.0f, fUpDown));
			}
			else if (ModifierKeys.HasFlag(Keys.Shift))
			{
				fLeftRight -= Math.Sign(e.Delta) * 0.2f;
				fLeftRight = Math.Max(-5, Math.Min(5.0f, fLeftRight));
			}
			else if (ModifierKeys.HasFlag(Keys.Alt))
			{
				fRotation += Math.Sign(e.Delta) * 0.1f;
				fRotation %= 360;
			}
			else
			{
				fZoom += Math.Sign(e.Delta);
				fZoom = Math.Max(-0.1f, Math.Min(17.0f, fZoom));
			}
		}

		private void DisposeModels()
		{
			if (pModel != null)
			{
				foreach (var model in pModel)
					model.Dispose();

				pModel.Clear();
			}
		}

		public void SetModel(string strFilePathA, string strEntityDistance, int nWearingPosition)
		{
			DisposeModels();

			if (strEntityDistance == "small")
				fUpDown = -0.345f;
			else if (strEntityDistance == "big")
				fUpDown = -1.5f;

			fLeftRight = 0.0f;

			strFilePath = strFilePathA;

			MakeLCModels(strFilePath, nWearingPosition);

			if (strEntityDistance == "big")
				fZoom += 5.0f;
		}

		private void CaptureScreenshot(string strFilePath)
		{
			Texture pCaptureTexture = new(pDevice, panel3DView.Width, panel3DView.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
			Surface pCaptureSurface = pCaptureTexture.GetSurfaceLevel(0);
			Surface oldRT = pDevice.GetRenderTarget(0);

			pDevice.SetRenderTarget(0, pCaptureSurface);

			pDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, new Color4(0f, 0f, 0f, 0f)/*transparent background*/, 1f, 0);
			pDevice.BeginScene();

			pDevice.SetTransform(TransformState.View, Matrix.LookAtLH(vecCameraPosition, vecEntityPosition, Vector3.UnitY));
			pDevice.SetTransform(TransformState.World, Matrix.RotationYawPitchRoll(fRotation, 3.1f, 0.0f));

			pDevice.SetRenderState(RenderState.AlphaTestEnable, true);
			pDevice.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
			pDevice.SetRenderState(RenderState.AlphaRef, 128);

			for (int i = 0; i < pModel?.Count; i++)
			{
				pDevice.SetTexture(0, pModel[i].TexData);

				pModel[i].MeshData.DrawSubset(0);
			}

			pDevice.EndScene();

			pDevice.SetRenderTarget(0, oldRT); oldRT.Dispose();

			SurfaceDescription pSurfaceDescription = pCaptureSurface.Description;

			using (Surface sysSurface = Surface.CreateOffscreenPlain(pDevice, pSurfaceDescription.Width, pSurfaceDescription.Height, pSurfaceDescription.Format, Pool.SystemMemory))
			{
				pDevice.GetRenderTargetData(pCaptureSurface, sysSurface);

				Surface.ToFile(sysSurface, strFilePath, ImageFileFormat.Png);
			}

			pCaptureTexture.Dispose();
			pCaptureSurface.Dispose();
		}

		private class tMesh : IDisposable
		{
			public Mesh MeshData;
			public Texture TexData;

			public tMesh(Mesh mesh, Texture texture)
			{
				MeshData = mesh;
				TexData = texture;
			}

			public void Dispose()
			{
				Dispose(true);
				//GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					MeshData?.Dispose();
					TexData?.Dispose();
				}
			}
		}

		private enum texFormat
		{
			RGB,
			ARGB,
			DXT1,
			DXT3,
			UNKNOWN
		}

		private class tHeader
		{
			public int AnimOffset;
			public uint Bits;
			public byte[]? DataChunk;
			public string? Format;
			public uint Height;
			public uint MipMap;
			public uint Shift;
			public uint Unknown;
			public int Version;
			public byte[]? VersionChunk;
			public uint Width;
		}

		private class tTexture
		{
			public tHeader? Header;
			public byte[][]? imageData;
		}

		private class Tex
		{
			public static tTexture? lcTex;

			public static texFormat GetFormat()
			{
				texFormat texFormat = texFormat.UNKNOWN;

				if (lcTex?.Header?.Format == "FRMC")
					return (int)lcTex.Header.Bits == 4 || (int)lcTex.Header.Bits == 13 ? texFormat.DXT1 : texFormat.DXT3;

				if (!(lcTex?.Header?.Format == "FRMS"))
					return texFormat;

				return (int)lcTex.Header.Bits == 0 || (int)lcTex.Header.Bits == 2 ? texFormat.RGB : texFormat.ARGB;
			}

			public static Bitmap makeRGB(byte[] data2, int width, int height)
			{
				List<byte> source = new();
				int index = 0;
				while (index < data2.Length)
				{
					source.Add(data2[index + 2]);
					source.Add(data2[index + 1]);
					source.Add(data2[index]);
					source.Add(byte.MaxValue);
					index += 4;
				}

				try
				{
					Bitmap bitmap = new(width, height, PixelFormat.Format32bppArgb);
					BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
					Marshal.Copy(source.ToArray(), 0, bitmapdata.Scan0, source.Count());
					bitmap.UnlockBits(bitmapdata);
					return bitmap;
				}
				catch
				{
					return new Bitmap(width, height);
				}
			}

			public static Bitmap makeRGB8(byte[] data2, int width, int height)
			{
				List<byte> source = new();
				int index = 0;
				while (index < data2.Length)
				{
					source.Add(data2[index + 1]);
					source.Add(data2[index]);
					source.Add(data2[index + 2]);
					index += 3;
				}

				try
				{
					Bitmap bitmap = new(width, height, PixelFormat.Format24bppRgb);
					BitmapData bitmapdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
					Marshal.Copy(source.ToArray(), 0, bitmapdata.Scan0, source.Count());
					bitmap.UnlockBits(bitmapdata);
					return bitmap;
				}
				catch
				{
					return new Bitmap(width, height);
				}
			}

			public static void ReadFile(string FileName)
			{
				ASCIIEncoding asciiEncoding = new();
				FileStream fileStream = new(FileName, FileMode.Open);
				BinaryReader b = new(fileStream);
				lcTex = new tTexture();
				lcTex.Header = new tHeader();
				lcTex.Header.VersionChunk = b.ReadBytes(4);
				lcTex.Header.Version = b.ReadInt32();
				lcTex.Header.DataChunk = b.ReadBytes(4);
				lcTex.Header.Width = b.ReadUInt32() ^ 303316286U;
				lcTex.Header.Shift = b.ReadUInt32() ^ 1431797889U;
				lcTex.Header.Height = b.ReadUInt32() ^ 2560279492U;
				lcTex.Header.MipMap = b.ReadUInt32() ^ 3688695303U;
				lcTex.Header.Bits = b.ReadUInt32() ^ 505432394U;
				lcTex.Header.Unknown = b.ReadUInt32() ^ 1633913997U;
				lcTex.Header.Format = asciiEncoding.GetString(b.ReadBytes(4));
				lcTex.Header.AnimOffset = b.ReadInt32();
				lcTex.Header.Width = Shift(lcTex.Header.Width, lcTex.Header.Shift);
				lcTex.Header.Height = Shift(lcTex.Header.Height, lcTex.Header.Shift);

				if (lcTex.Header.Format == "FRMC")
					ReadFRMC(lcTex, b);
				else if (lcTex.Header.Format == "FRMS")
					ReadFRMS(lcTex, b);

				fileStream.Close();
			}

			private static void ReadFRMC(tTexture lcTex, BinaryReader b)
			{
				try
				{
					int mipMapCount = (int)lcTex.Header.MipMap;

					if (mipMapCount >= 0)
					{
						lcTex.imageData = new byte[mipMapCount][];

						for (int index = 0; index < mipMapCount; ++index)
						{
							if (b.BaseStream.Length - b.BaseStream.Position >= sizeof(int))
							{
								int mipMapSize = b.ReadInt32();

								if (mipMapSize >= 0 && b.BaseStream.Length - b.BaseStream.Position >= mipMapSize)
								{
									lcTex.imageData[index] = b.ReadBytes(mipMapSize);
								}
								else
								{
									pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Mipmap size out of range of index: {index} (SMC: {strFilePath}).");
									lcTex.imageData = null;
									break;
								}
							}
							else
							{
								pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Index of Mipmap out of range (SMC: {strFilePath}).");
								lcTex.imageData = null;
								break;
							}
						}
					}
					else
					{
						pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Mipmap number out of range (SMC: {strFilePath}).");
						lcTex.imageData = null;
					}
				}
				catch (Exception ex)
				{
					pMain.Logger(LogTypes.Error, $"3DViewer Dialog > {ex.Message} (SMC: {strFilePath}).");
					lcTex.imageData = null;
				}
			}

			private static void ReadFRMS(tTexture lcTex, BinaryReader b)
			{
				int num = (int)lcTex.Header.Width * (int)lcTex.Header.Height;
				int count = (int)lcTex.Header.Bits == 0 || (int)lcTex.Header.Bits == 2 ? num * 3 : num * 4;

				if (lcTex.Header.MipMap > int.MaxValue)
					return;

				lcTex.imageData = new byte[(int)lcTex.Header.MipMap][];

				for (int index = 0; index < lcTex.Header.MipMap; ++index)
					lcTex.imageData[index] = b.ReadBytes(count);
			}

			private static uint Shift(uint Val, uint Shifter)
			{
				Val >>= (int)Shifter;
				return Val;
			}
		}

		private struct smcObject
		{
			public string Name;
			public string Texture;

			public smcObject(string Name, string Texture)
			{
				this.Name = Name;
				this.Texture = Texture;
			}
		}

		private struct smcMesh
		{
			public string FileName;
			public List<smcObject> Object;

			public smcMesh(string FileName)
			{
				this.FileName = FileName;
				Object = new List<smcObject>();
			}
		}

		private class SMCReader
		{
			public static List<smcMesh> ReadFile(string FileName)
			{
				string[] strArray1 = (Path.GetDirectoryName(FileName) ?? string.Empty).Split('\\');
				string str = "";
				bool flag = true;

				for (int i = 0; i < (strArray1).Count(); ++i)
				{
					if (strArray1[i].ToUpper() == "DATA")
						flag = false;

					if (flag)
						str = str + strArray1[i] + "\\";
				}

				List<string> list = (File.ReadAllLines(FileName)).ToList();
				for (int i = list.Count() - 1; i >= 0; --i)
				{
					list[i] = list[i].Trim();
					list[i] = list[i].Replace("TFNM", "");
					if (list[i].Contains("{") || list[i].Contains("}") || (list[i].Contains(",") || list[i].Contains("NAME")) || (list[i].Contains("COLISION") || list[i].Contains("TEXTURES") || (list[i].Contains("ANIM") || list[i].Contains("SKELETON"))) || list[i].Contains("_TAG"))
						list.RemoveAt(i);
				}

				int index1 = -1;
				List<smcMesh> smcMeshList = new();
				for (int i = 0; i < list.Count; ++i)
				{
					if (list[i].Length >= 4 && list[i].Substring(0, 4) == "MESH")
					{
						++index1;
						string[] strArray2 = list[i].Split('"');

						if (strArray2.Length > 1)
							smcMeshList.Add(new smcMesh(str + strArray2[1]));
						else
							pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Wrong MESH format (SMC: {strFilePath}).");
					}
					else
					{
						if (index1 >= 0)
						{
							string[] strArray2 = list[i].Split('"');

							if (strArray2.Length > 3)
								smcMeshList[index1].Object.Add(new smcObject(strArray2[1], str + strArray2[3]));
							else
								pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Something is wrong in (Very useful log hu) (SMC: {strFilePath}).");
						}
					}
				}

				return smcMeshList;
			}
		}

		private struct tMeshMorphMap
		{
			public byte[] JIndex;
			public byte[] Influence;

			public tMeshMorphMap(byte[] JIndex, byte[] Influence)
			{
				this.JIndex = JIndex;
				this.Influence = Influence;
			}
		}

		private class tHeaderInfo
		{
			public byte[]? Format;
			public uint JointCount;
			public uint MeshCount;
			public int MeshDataSize;
			public uint NormalCount;
			public uint ObjCount;
			public uint TextureMaps;
			public uint UnknownCount;
			public int Version;
			public uint VertexCount;
		}

		private struct tVertex3f
		{
			public float X;
			public float Y;
			public float Z;

			public tVertex3f(float X, float Y, float Z)
			{
				this.X = X;
				this.Y = Y;
				this.Z = Z;
			}
		}

		private class tFace
		{
			public short A;
			public short B;
			public short C;

			public tFace(short A, short B, short C)
			{
				this.A = A;
				this.B = B;
				this.C = C;
			}
		}

		private class tMeshShaderData : IDisposable
		{
			public uint cParam0;
			public uint[]? Param1;
			public uint[]? Param2;
			public float[]? ParamFloats;
			public byte[]? ShaderName;

			public void Dispose()
			{
				Param1 = null;
				Param2 = null;
				ParamFloats = null;
				ShaderName = null;
			}
		}

		private class tMeshShaderInfo
		{
			public uint cParam1;
			public uint cParam2;
			public uint cParamFloats;
			public uint cTextureUnits;
		}

		private struct tMeshTexture
		{
			public byte[] InternalName;
			public int Reserverd;

			public tMeshTexture(byte[] Name)
			{
				InternalName = Name;
				Reserverd = 0;
			}
		}

		private struct tTextCoord
		{
			public float U;
			public float V;

			public tTextCoord(float U, float V)
			{
				this.U = U;
				this.V = V;
			}
		}

		private class tMeshUVMap
		{
			public tTextCoord[]? Coords;
			public byte[]? Name;
		}

		private struct tMeshWeightsMap
		{
			public int Index;
			public float Weight;

			public tMeshWeightsMap(int Index, float Weight)
			{
				this.Index = Index;
				this.Weight = Weight;
			}
		}

		private class tMeshJointWeights
		{
			public uint Count;
			public byte[]? JointName;
			public tMeshWeightsMap[]? WeightsMap;
		}

		public struct tVertexWeightInfo
		{
			public byte[] Indices;
			public byte[] Weights;
		}

		private class tMeshObject : IDisposable
		{
			public uint FaceCount;
			public tFace[]? Faces;
			public uint FromVert;
			public byte[]? JData;
			public uint JValue;
			public byte[]? MaterialName;
			public tMeshShaderData? ShaderData;
			public uint ShaderFlag;
			public tMeshShaderInfo? ShaderInfo;
			public tMeshTexture[]? Textures;
			public uint ToVert;
			public uint Value1;

			public short[] GetFaces()
			{
				List<short> shortList = new();
				for (int i = 0; i < Faces?.Length; ++i)
				{
					shortList.Add(Faces[i].A);
					shortList.Add(Faces[i].B);
					shortList.Add(Faces[i].C);
				}

				return shortList.ToArray();
			}

			public void Dispose()
			{
				ShaderData?.Dispose();
			}
		}

		private class tMeshContainer
		{
			public byte[]? FileName;
			public string? FilePath;
			public tHeaderInfo? HeaderInfo;
			public tMeshMorphMap[]? MorphMap;
			public tVertex3f[]? Normals;
			public tMeshObject[]? Objects;
			public float Scale;
			public tMeshUVMap[]? UVMaps;
			public uint Value1;
			public tVertex3f[]? Vertices;
			public tMeshJointWeights[]? Weights;
			public tVertexWeightInfo[]? VertexWeights;
		}

		private class LCMeshReader
		{
			public static bool ReadFile(string FileName)
			{
				pMesh = new tMeshContainer();
				BinaryReader b = new BinaryReader(new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read));
				pMesh.HeaderInfo = new tHeaderInfo();
				pMesh.HeaderInfo.Format = b.ReadBytes(4);
				pMesh.HeaderInfo.Version = b.ReadInt32();
				pMesh.HeaderInfo.MeshDataSize = b.ReadInt32();
				pMesh.HeaderInfo.MeshCount = b.ReadUInt32();
				pMesh.HeaderInfo.VertexCount = b.ReadUInt32();
				pMesh.HeaderInfo.JointCount = b.ReadUInt32();
				pMesh.HeaderInfo.TextureMaps = b.ReadUInt32();
				pMesh.HeaderInfo.NormalCount = b.ReadUInt32();
				pMesh.HeaderInfo.ObjCount = b.ReadUInt32();
				pMesh.HeaderInfo.UnknownCount = b.ReadUInt32();
				pMesh.FileName = b.ReadBytes(b.ReadInt32());
				pMesh.Scale = b.ReadSingle();
				pMesh.Value1 = b.ReadUInt32();
				pMesh.FilePath = FileName;

				bool flag = false;

				if (pMesh.HeaderInfo.Version == 16 && ReadV10(b, b.BaseStream.Position))
					flag = true;
				else if (pMesh.HeaderInfo.Version == 17 && ReadV11(b, b.BaseStream.Position))
					flag = true;

				b.Close();

				return flag;
			}

			private static bool ReadV10(BinaryReader b, long Pos)
			{
				uint vtxCount = pMesh.HeaderInfo.VertexCount;
				uint normalCount = pMesh.HeaderInfo.NormalCount;
				uint jointCount = pMesh.HeaderInfo.JointCount;
				uint texMapCount = pMesh.HeaderInfo.TextureMaps;

				tHeaderInfo headerInfo = pMesh.HeaderInfo;
				headerInfo.NormalCount = vtxCount;
				headerInfo.JointCount = normalCount;
				headerInfo.TextureMaps = jointCount;
				headerInfo.UnknownCount = texMapCount;
				headerInfo.ObjCount = pMesh.HeaderInfo.ObjCount;

				pMesh.HeaderInfo = headerInfo;

				pMesh.Vertices = new tVertex3f[(int)pMesh.HeaderInfo.VertexCount];

				for (int index = 0; index < pMesh.HeaderInfo.VertexCount; ++index)
					pMesh.Vertices[index] = new tVertex3f(b.ReadSingle(), b.ReadSingle(), b.ReadSingle());

				pMesh.Normals = new tVertex3f[(int)pMesh.HeaderInfo.VertexCount];

				for (int index = 0; index < pMesh.HeaderInfo.VertexCount; ++index)
					pMesh.Normals[index] = new tVertex3f(b.ReadSingle(), b.ReadSingle(), b.ReadSingle());

				if (pMesh.HeaderInfo.TextureMaps > 0U)
				{
					pMesh.UVMaps = new tMeshUVMap[(int)pMesh.HeaderInfo.TextureMaps];

					for (int index1 = 0; index1 < pMesh.HeaderInfo.TextureMaps; ++index1)
					{
						pMesh.UVMaps[index1] = new tMeshUVMap();
						pMesh.UVMaps[index1].Name = b.ReadBytes(b.ReadInt32());
						pMesh.UVMaps[index1].Coords = new tTextCoord[(int)pMesh.HeaderInfo.VertexCount];

						for (int index2 = 0; index2 < pMesh.HeaderInfo.VertexCount; ++index2)
							pMesh.UVMaps[index1].Coords[index2] = new tTextCoord(b.ReadSingle(), b.ReadSingle());
					}
				}

				pMesh.Objects = new tMeshObject[(int)pMesh.HeaderInfo.ObjCount];

				for (int index1 = 0; index1 < pMesh.HeaderInfo.ObjCount; ++index1)
				{
					tMeshObject tMeshObject = new();
					tMeshObject.MaterialName = b.ReadBytes(b.ReadInt32());
					tMeshObject.Value1 = b.ReadUInt32();
					tMeshObject.FromVert = b.ReadUInt32();
					tMeshObject.ToVert = b.ReadUInt32();
					tMeshObject.FaceCount = b.ReadUInt32();
					tMeshObject.Faces = new tFace[(int)tMeshObject.FaceCount];

					for (int index2 = 0; index2 < tMeshObject.FaceCount; ++index2)
						tMeshObject.Faces[index2] = new tFace(b.ReadInt16(), b.ReadInt16(), b.ReadInt16());

					tMeshObject.JValue = b.ReadUInt32();
					tMeshObject.JData = new byte[(int)tMeshObject.JValue];

					for (int index2 = 0; index2 < tMeshObject.JValue; ++index2)
						tMeshObject.JData[index2] = b.ReadByte();

					tMeshObject.ShaderFlag = b.ReadUInt32();

					if (tMeshObject.ShaderFlag > 0U)
					{
						tMeshObject.ShaderInfo = new tMeshShaderInfo();
						tMeshObject.ShaderInfo.cParam1 = b.ReadUInt32();
						tMeshObject.ShaderInfo.cParamFloats = b.ReadUInt32();
						tMeshObject.ShaderInfo.cTextureUnits = b.ReadUInt32();
						tMeshObject.ShaderInfo.cParam2 = b.ReadUInt32();
						tMeshObject.ShaderInfo = new tMeshShaderInfo()
						{
							cTextureUnits = tMeshObject.ShaderInfo.cParam1,
							cParamFloats = tMeshObject.ShaderInfo.cParamFloats,
							cParam1 = tMeshObject.ShaderInfo.cParam2,
							cParam2 = tMeshObject.ShaderInfo.cTextureUnits
						};

						tMeshObject.ShaderData = new tMeshShaderData();
						tMeshObject.ShaderData.ShaderName = b.ReadBytes(b.ReadInt32());
						tMeshObject.Textures = new tMeshTexture[(int)tMeshObject.ShaderInfo.cTextureUnits];

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cTextureUnits; ++index2)
							tMeshObject.Textures[index2] = new tMeshTexture(b.ReadBytes(b.ReadInt32()));

						if (tMeshObject.ShaderInfo.cParam1 > 0U)
							tMeshObject.ShaderData.Param1 = new uint[(int)tMeshObject.ShaderInfo.cParam1];

						if (tMeshObject.ShaderInfo.cParamFloats > 0U)
							tMeshObject.ShaderData.ParamFloats = new float[(int)tMeshObject.ShaderInfo.cParamFloats];

						if (tMeshObject.ShaderInfo.cParam2 > 0U)
							tMeshObject.ShaderData.Param2 = new uint[(int)tMeshObject.ShaderInfo.cParam2];

						tMeshObject.ShaderData.cParam0 = b.ReadUInt32();

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParam2; ++index2)
							tMeshObject.ShaderData.Param2[index2] = b.ReadUInt32();

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParamFloats; ++index2)
							tMeshObject.ShaderData.ParamFloats[index2] = b.ReadSingle();

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParam1; ++index2)
							tMeshObject.ShaderData.Param1[index2] = b.ReadUInt32();
					}

					pMesh.Objects[index1] = tMeshObject;
				}

				if (pMesh.HeaderInfo.VertexCount > 0 && pMesh.HeaderInfo.JointCount > 0)
				{
					pMesh.VertexWeights = new tVertexWeightInfo[(int)pMesh.HeaderInfo.VertexCount];

					for (int i = 0; i < pMesh.HeaderInfo.VertexCount; i++)
						pMesh.VertexWeights[i] = new tVertexWeightInfo { Indices = b.ReadBytes(4), Weights = b.ReadBytes(4) };
				}

				pMesh.MorphMap = new tMeshMorphMap[(int)pMesh.HeaderInfo.UnknownCount];

				for (int index = 0; index < pMesh.HeaderInfo.UnknownCount; ++index)
					pMesh.MorphMap[index] = new tMeshMorphMap(b.ReadBytes(4), b.ReadBytes(4));

				return b.BaseStream.Position == (pMesh.HeaderInfo.MeshDataSize + 8);
			}

			private class Decoder
			{
				private static byte[] XorCode = new byte[4] { 101, 72, 53, 30 };

				public static uint Decode(uint Code)
				{
					byte[] bytes = BitConverter.GetBytes(Code);

					for (int index = 0; index < 4; ++index)
					{
						bytes[index] = (byte)(bytes[index] ^ XorCode[index]);
						XorCode[index] = (byte)(XorCode[index] + 89U);
					}

					return BitConverter.ToUInt32(bytes, 0);
				}

				public static void Reset() { XorCode = new byte[4] { 101, 72, 53, 30 }; }
			}

			private static bool ReadV11(BinaryReader b, long Pos)
			{
				b.BaseStream.Position = Pos;

				Decoder.Reset();

				pMesh.HeaderInfo.MeshCount = Decoder.Decode(pMesh.HeaderInfo.MeshCount);
				pMesh.HeaderInfo.VertexCount = Decoder.Decode(pMesh.HeaderInfo.VertexCount);
				pMesh.HeaderInfo.JointCount = Decoder.Decode(pMesh.HeaderInfo.JointCount);
				pMesh.HeaderInfo.TextureMaps = Decoder.Decode(pMesh.HeaderInfo.TextureMaps);
				pMesh.HeaderInfo.NormalCount = Decoder.Decode(pMesh.HeaderInfo.NormalCount);
				pMesh.HeaderInfo.ObjCount = Decoder.Decode(pMesh.HeaderInfo.ObjCount);
				pMesh.HeaderInfo.UnknownCount = Decoder.Decode(pMesh.HeaderInfo.UnknownCount);
				pMesh.Value1 = Decoder.Decode(pMesh.Value1);
				pMesh.Vertices = new tVertex3f[(int)pMesh.HeaderInfo.VertexCount];

				for (int index = 0; index < pMesh.HeaderInfo.VertexCount; ++index)
					pMesh.Vertices[index] = new tVertex3f(b.ReadSingle(), b.ReadSingle(), b.ReadSingle());

				pMesh.Normals = new tVertex3f[(int)pMesh.HeaderInfo.NormalCount];

				for (int index = 0; index < pMesh.HeaderInfo.NormalCount; ++index)
					pMesh.Normals[index] = new tVertex3f(b.ReadSingle(), b.ReadSingle(), b.ReadSingle());

				if (pMesh.HeaderInfo.TextureMaps > 0U)
				{
					pMesh.UVMaps = new tMeshUVMap[(int)pMesh.HeaderInfo.TextureMaps];

					for (int index1 = 0; index1 < pMesh.HeaderInfo.TextureMaps; ++index1)
					{
						tMeshUVMap tMeshUvMap = new();
						tMeshUvMap.Name = b.ReadBytes(b.ReadInt32());
						tMeshUvMap.Coords = new tTextCoord[(int)pMesh.HeaderInfo.VertexCount];

						for (int index2 = 0; index2 < pMesh.HeaderInfo.VertexCount; ++index2)
							tMeshUvMap.Coords[index2] = new tTextCoord(b.ReadSingle(), b.ReadSingle());

						pMesh.UVMaps[index1] = tMeshUvMap;
					}
				}

				pMesh.Objects = new tMeshObject[(int)pMesh.HeaderInfo.ObjCount];

				for (int index1 = 0; index1 < pMesh.HeaderInfo.ObjCount; ++index1)
				{
					tMeshObject tMeshObject = new tMeshObject();
					tMeshObject.FromVert = Decoder.Decode(b.ReadUInt32());
					tMeshObject.ToVert = Decoder.Decode(b.ReadUInt32());
					tMeshObject.FaceCount = Decoder.Decode(b.ReadUInt32());
					tMeshObject.Faces = new tFace[(int)tMeshObject.FaceCount];

					for (int index2 = 0; index2 < tMeshObject.FaceCount; ++index2)
						tMeshObject.Faces[index2] = new tFace(b.ReadInt16(), b.ReadInt16(), b.ReadInt16());

					tMeshObject.MaterialName = b.ReadBytes(b.ReadInt32());
					tMeshObject.Value1 = Decoder.Decode(b.ReadUInt32());
					tMeshObject.JValue = Decoder.Decode(b.ReadUInt32());
					tMeshObject.JData = new byte[(int)tMeshObject.JValue];

					for (int index2 = 0; index2 < tMeshObject.JValue; ++index2)
						tMeshObject.JData[index2] = b.ReadByte();

					tMeshObject.ShaderFlag = Decoder.Decode(b.ReadUInt32());

					if (tMeshObject.ShaderFlag > 0U)
					{
						tMeshObject.ShaderInfo = new tMeshShaderInfo();
						tMeshObject.ShaderInfo.cParam1 = Decoder.Decode(b.ReadUInt32());
						tMeshObject.ShaderInfo.cParamFloats = Decoder.Decode(b.ReadUInt32());
						tMeshObject.ShaderInfo.cTextureUnits = Decoder.Decode(b.ReadUInt32());
						tMeshObject.ShaderInfo.cParam2 = Decoder.Decode(b.ReadUInt32());
						tMeshObject.ShaderData = new tMeshShaderData();
						tMeshObject.ShaderData.ShaderName = b.ReadBytes(b.ReadInt32());
						tMeshObject.Textures = new tMeshTexture[(int)tMeshObject.ShaderInfo.cTextureUnits];

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cTextureUnits; ++index2)
						{
							tMeshObject.Textures[index2] = new tMeshTexture();
							tMeshObject.Textures[index2].InternalName = b.ReadBytes(b.ReadInt32());
						}

						if (tMeshObject.ShaderInfo.cParam2 > 0U)
							tMeshObject.ShaderData.Param1 = new uint[(int)tMeshObject.ShaderInfo.cParam1];

						if (tMeshObject.ShaderInfo.cParamFloats > 0U)
							tMeshObject.ShaderData.ParamFloats = new float[(int)tMeshObject.ShaderInfo.cParamFloats];

						if (tMeshObject.ShaderInfo.cParam1 > 0U)
							tMeshObject.ShaderData.Param2 = new uint[(int)tMeshObject.ShaderInfo.cParam2];

						tMeshObject.ShaderData.cParam0 = Decoder.Decode(b.ReadUInt32());

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParam2; ++index2)
							tMeshObject.ShaderData.Param2[index2] = Decoder.Decode(b.ReadUInt32());

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParamFloats; ++index2)
							tMeshObject.ShaderData.ParamFloats[index2] = b.ReadSingle();

						for (int index2 = 0; index2 < tMeshObject.ShaderInfo.cParam1; ++index2)
							tMeshObject.ShaderData.Param1[index2] = Decoder.Decode(b.ReadUInt32());
					}

					pMesh.Objects[index1] = tMeshObject;
				}

				pMesh.Weights = new tMeshJointWeights[(int)pMesh.HeaderInfo.JointCount];

				for (int index1 = 0; index1 < pMesh.HeaderInfo.JointCount; ++index1)
				{
					pMesh.Weights[index1] = new tMeshJointWeights();
					pMesh.Weights[index1].JointName = b.ReadBytes(b.ReadInt32());
					pMesh.Weights[index1].Count = Decoder.Decode(b.ReadUInt32());
					pMesh.Weights[index1].WeightsMap = new tMeshWeightsMap[(int)pMesh.Weights[index1].Count];

					for (int index2 = 0; index2 < pMesh.Weights[index1].Count; ++index2)
						pMesh.Weights[index1].WeightsMap[index2] = new tMeshWeightsMap(b.ReadInt32(), b.ReadSingle());
				}

				pMesh.MorphMap = new tMeshMorphMap[(int)pMesh.HeaderInfo.VertexCount];

				for (int index = 0; index < pMesh.HeaderInfo.VertexCount; ++index)
					pMesh.MorphMap[index] = new tMeshMorphMap(b.ReadBytes(4), b.ReadBytes(4));

				Pos = b.BaseStream.Position;

				return Pos == (pMesh.HeaderInfo.MeshDataSize + 8);
			}
		}

		private static Format ConvFormat(texFormat tFormat)
		{
			Format format = Format.Unknown;
			switch (tFormat)
			{
				case texFormat.RGB:
					return Format.R8G8B8;
				case texFormat.ARGB:
					return Format.A8R8G8B8;
				case texFormat.DXT1:
					return Format.Dxt1;
				case texFormat.DXT3:
					return Format.Dxt3;
				default:
					return format;
			}
		}

		private Texture BuildTexture(byte[] imageData, Format imageFormat, int width, int height)
		{
			if (imageFormat == Format.R8G8B8)
			{
				using (MemoryStream memoryStream = new MemoryStream())
				{
					Tex.makeRGB8(imageData, width, height).Save(memoryStream, ImageFormat.Bmp);
					memoryStream.Write(imageData, 0, imageData.Length);
					memoryStream.Position = 0L;

					return Texture.FromStream(pDevice, memoryStream, width, height, 0, Usage.SoftwareProcessing, Format.A8B8G8R8, Pool.Default, Filter.None, Filter.None, 0);
				}
			}
			else if (imageFormat == Format.A8R8G8B8)
			{
				using (MemoryStream memoryStream = new())
				{
					Tex.makeRGB(imageData, width, height).Save(memoryStream, ImageFormat.Bmp);
					memoryStream.Write(imageData, 0, imageData.Length);
					memoryStream.Position = 0L;

					return Texture.FromStream(pDevice, memoryStream, width, height, 0, Usage.SoftwareProcessing, Format.A8B8G8R8, Pool.Default, Filter.None, Filter.None, 0);
				}
			}
			else
			{
				Texture texture = new Texture(pDevice, width, height, 0, Usage.None, imageFormat, Pool.Managed);
				using (Stream data = texture.LockRectangle(0, LockFlags.None).Data)
				{
					data.Write(imageData, 0, (imageData).Count<byte>());
					texture.UnlockRectangle(0);
				}

				return texture;
			}
		}

		private Texture? GetTextureFromFile(string FileName)
		{
			Texture? texture = null;

			if (File.Exists(FileName))
			{
				Tex.ReadFile(FileName);

				if (Tex.lcTex.imageData != null && Tex.lcTex.imageData.Length > 0 && Tex.lcTex.imageData[0] != null)
					texture = BuildTexture(Tex.lcTex.imageData[0], ConvFormat(Tex.GetFormat()), (int)Tex.lcTex.Header.Width, (int)Tex.lcTex.Header.Height);
				else
					pMain.Logger(LogTypes.Error, $"3DViewer Dialog > imageData is empty or null (SMC: {strFilePath}).");
			}

			return texture;
		}

		private struct PositionNormalTextured
		{
			public Vector3 Position;
			public Vector3 Normal;
			public Vector2 Texture;
		}

		private void MakeLCModels(string SMCFile, int nWearingPosition)
		{
			List<smcMesh> list = SMCReader.ReadFile(SMCFile);
			pModel = new List<tMesh>();

			for (int i = 0; i < list.Count; i++)
			{
				bool flag =
					(nWearingPosition != 0 || !list[i].FileName.Contains("_hair_000")) &&
					(nWearingPosition != 1 || !list[i].FileName.Contains("_bu_000")) &&
					(nWearingPosition != 3 || !list[i].FileName.Contains("_bd_000")) &&
					(nWearingPosition != 5 || !list[i].FileName.Contains("_hn_000")) &&
					(nWearingPosition != 6 || !list[i].FileName.Contains("_ft_000"));

				if (!flag)
					continue;

				if (!LCMeshReader.ReadFile(list[i].FileName))
					continue;

				for (int j = 0; j < pMesh.Objects.Length; j++)
				{
					int fromVert = (int)pMesh.Objects[j].FromVert;
					int toVert = (int)pMesh.Objects[j].ToVert;
					short[] faces = pMesh.Objects[j].GetFaces();
					PositionNormalTextured[] vertices = new PositionNormalTextured[toVert];

					for (int k = 0; k < toVert; k++)
					{
						int idx = fromVert + k;

						vertices[k].Position = new Vector3(pMesh.Vertices[idx].X, pMesh.Vertices[idx].Y, pMesh.Vertices[idx].Z);

						vertices[k].Normal = new Vector3(pMesh.Normals[idx].X, pMesh.Normals[idx].Y, pMesh.Normals[idx].Z);

						Vector2 uv = Vector2.Zero;

						if (pMesh.UVMaps != null && pMesh.UVMaps.Length > 0 && pMesh.UVMaps[0].Coords != null && idx < pMesh.UVMaps[0].Coords.Length)
							uv = new Vector2(pMesh.UVMaps[0].Coords[idx].U, pMesh.UVMaps[0].Coords[idx].V);

						vertices[k].Texture = uv;
					}

					Mesh mesh = new Mesh(pDevice, faces.Length / 3, vertices.Length, MeshFlags.Managed, VertexFormat.Position | VertexFormat.Normal | VertexFormat.Texture1);

					using (DataStream ds = mesh.VertexBuffer.Lock(0, 0, LockFlags.None))
						ds.WriteRange(vertices);

					mesh.VertexBuffer.Unlock();

					using (DataStream ds = mesh.IndexBuffer.Lock(0, 0, LockFlags.None))
						ds.WriteRange(faces);

					mesh.IndexBuffer.Unlock();

					mesh.GenerateAdjacency(0.5f);

					Texture? texture = pMissingTexture;

					if (pMesh.Objects[j].Textures != null && pMesh.Objects[j].Textures.Length > 0)
					{
						int texIndex = list[i].Object.FindIndex(x => x.Name.Equals(pEnc.GetString(pMesh.Objects[j].Textures[0].InternalName), StringComparison.OrdinalIgnoreCase));

						if (texIndex != -1)
							texture = GetTextureFromFile(list[i].Object[texIndex].Texture);
					}

					pModel.Add(new tMesh(mesh, texture));
				}
			}

			fZoom = 4f;
		}

		private void InitializeDevice()
		{
			pPresentParameters = new PresentParameters();

			pPresentParameters.SwapEffect = SwapEffect.Discard;
			pPresentParameters.DeviceWindowHandle = panel3DView.Handle;
			pPresentParameters.Windowed = 1 != 0;
			pPresentParameters.BackBufferWidth = panel3DView.Width;
			pPresentParameters.BackBufferHeight = panel3DView.Height;
			pPresentParameters.BackBufferFormat = (Format)21;

			pDevice = new Device(new Direct3D(), 0, (DeviceType)1, Handle, (CreateFlags)32, pPresentParameters);
			pDevice.SetRenderState(RenderState.CullMode, Cull.None);
			pDevice.SetRenderState(RenderState.FillMode, FillMode.Solid);
			pDevice.SetRenderState(RenderState.Lighting, false);
			// FIX out of range error
			pMissingTexture = new Texture(pDevice, 1, 1, 1, Usage.None, Format.A8R8G8B8, Pool.Managed);

			DataRectangle pRect = pMissingTexture.LockRectangle(0, LockFlags.None);
			pRect.Data.Write(0xFFFF00FF);
			pMissingTexture.UnlockRectangle(0);
			/****************************************/
			pDevice.SetTransform(TransformState.Projection, Matrix.PerspectiveFovLH(100f, (float)panel3DView.Width / panel3DView.Height, 1f, 100f));

			fZoom = fRotation = 0.0f;
		}

		private void timerRender_Tick(object? sender, EventArgs e)
		{
			pDevice.Viewport = new Viewport(0, 0, panel3DView.Width, panel3DView.Height);

			if (bDeviceReset)
			{
				pDevice.SetRenderState(RenderState.Lighting, false);
				pDevice.SetRenderState(RenderState.ZEnable, true);
				pDevice.SetRenderState(RenderState.CullMode, Cull.None);

				bDeviceReset = false;
			}

			pDevice.Clear(ClearFlags.ZBuffer | ClearFlags.Target, Color.FromArgb(0, 0, 0), 1f, 0);
			pDevice.BeginScene();

			vecCameraPosition.Z = fZoom;
			vecEntityPosition.Y = fUpDown;
			vecEntityPosition.X = fLeftRight;

			pDevice.SetTransform(TransformState.View, Matrix.LookAtLH(vecCameraPosition, vecEntityPosition, Vector3.UnitY));
			pDevice.SetTransform(TransformState.World, Matrix.RotationYawPitchRoll(fRotation, 3.1f, 0.0f));

			if (pModel != null)
			{
				// FIX Alpha Textures
				if (cbAlpha.Checked)
				{
					pDevice.SetRenderState(RenderState.AlphaTestEnable, true);
					pDevice.SetRenderState(RenderState.AlphaFunc, Compare.Greater);
					pDevice.SetRenderState(RenderState.AlphaRef, 128);
				}
				else
				{
					pDevice.SetRenderState(RenderState.AlphaTestEnable, false);
				}
				/****************************************/
				pDevice.SetTexture(0, null);
				pDevice.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Color1);

				for (int index = 0; index < pModel.Count; ++index)
				{
					if (pModel[index].TexData != null)
						pDevice.SetTexture(0, pModel[index].TexData);

					pModel[index].MeshData.DrawSubset(0);
				}
			}

			pDevice.EndScene();

			if (bCaptureShot)
			{
				bool bSuccess = true;
				string strCaptureFileName = $"3DViewer Captures\\{Path.GetFileNameWithoutExtension(strFilePath)}_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.png";

				try
				{
					if (!Directory.Exists("3DViewer Captures"))
						Directory.CreateDirectory("3DViewer Captures");

					CaptureScreenshot(strCaptureFileName);
				}
				catch (Exception ex)
				{
					pMain.Logger(LogTypes.Error, $"3DViewer Dialog > Something got wrong when try to save capture Error: {ex.Message}.");

					bSuccess = false;
				}
				finally
				{
					if (bSuccess)
						pMain.Logger(LogTypes.Success, "3DViewer Dialog > Capture Saved to: " + strCaptureFileName);
				}

				bCaptureShot = false;
			}

			pDevice.Present();

			if (cbRotation.Checked)
				fRotation = fRotation - 0.01f;
		}
	}
}