﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Pathfinding.Ionic.Zip;
using Pathfinding.Util;
using UnityEngine;

namespace Pathfinding.Serialization
{
	
	public class AstarSerializer
	{
		
		public AstarSerializer(AstarData data)
		{
			this.data = data;
			this.settings = SerializeSettings.Settings;
		}

		
		public AstarSerializer(AstarData data, SerializeSettings settings)
		{
			this.data = data;
			this.settings = settings;
		}

		
		private static StringBuilder GetStringBuilder()
		{
			AstarSerializer._stringBuilder.Length = 0;
			return AstarSerializer._stringBuilder;
		}

		
		private static void CloseOrDispose(BinaryWriter writer)
		{
			writer.Close();
		}

		
		public void SetGraphIndexOffset(int offset)
		{
			this.graphIndexOffset = offset;
		}

		
		private void AddChecksum(byte[] bytes)
		{
			this.checksum = Checksum.GetChecksum(bytes, this.checksum);
		}

		
		public uint GetChecksum()
		{
			return this.checksum;
		}

		
		public void OpenSerialize()
		{
			this.zip = new ZipFile();
			this.zip.AlternateEncoding = Encoding.UTF8;
			this.zip.AlternateEncodingUsage = ZipOption.Always;
			this.meta = new GraphMeta();
		}

		
		public byte[] CloseSerialize()
		{
			byte[] array = this.SerializeMeta();
			this.AddChecksum(array);
			this.zip.AddEntry("meta.json", array);
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			foreach (ZipEntry zipEntry in this.zip.Entries)
			{
				zipEntry.AccessedTime = dateTime;
				zipEntry.CreationTime = dateTime;
				zipEntry.LastModified = dateTime;
				zipEntry.ModifiedTime = dateTime;
			}
			MemoryStream memoryStream = new MemoryStream();
			this.zip.Save(memoryStream);
			array = memoryStream.ToArray();
			memoryStream.Dispose();
			this.zip.Dispose();
			this.zip = null;
			return array;
		}

		
		public void SerializeGraphs(NavGraph[] _graphs)
		{
			if (this.graphs != null)
			{
				throw new InvalidOperationException("Cannot serialize graphs multiple times.");
			}
			this.graphs = _graphs;
			if (this.zip == null)
			{
				throw new NullReferenceException("You must not call CloseSerialize before a call to this function");
			}
			if (this.graphs == null)
			{
				this.graphs = new NavGraph[0];
			}
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					byte[] array = this.Serialize(this.graphs[i]);
					this.AddChecksum(array);
					this.zip.AddEntry("graph" + i + ".json", array);
				}
			}
		}

		
		private byte[] SerializeMeta()
		{
			if (this.graphs == null)
			{
				throw new Exception("No call to SerializeGraphs has been done");
			}
			this.meta.version = AstarPath.Version;
			this.meta.graphs = this.graphs.Length;
			this.meta.guids = new List<string>();
			this.meta.typeNames = new List<string>();
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					this.meta.guids.Add(this.graphs[i].guid.ToString());
					this.meta.typeNames.Add(this.graphs[i].GetType().FullName);
				}
				else
				{
					this.meta.guids.Add(null);
					this.meta.typeNames.Add(null);
				}
			}
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			TinyJsonSerializer.Serialize(this.meta, stringBuilder);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		
		public byte[] Serialize(NavGraph graph)
		{
			StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
			TinyJsonSerializer.Serialize(graph, stringBuilder);
			return this.encoding.GetBytes(stringBuilder.ToString());
		}

		
		[Obsolete("Not used anymore. You can safely remove the call to this function.")]
		public void SerializeNodes()
		{
		}

		
		private static int GetMaxNodeIndexInAllGraphs(NavGraph[] graphs)
		{
			int maxIndex = 0;
			for (int i = 0; i < graphs.Length; i++)
			{
				if (graphs[i] != null)
				{
					graphs[i].GetNodes(delegate(GraphNode node)
					{
						maxIndex = Math.Max(node.NodeIndex, maxIndex);
						if (node.NodeIndex == -1)
						{
							Debug.LogError("Graph contains destroyed nodes. This is a bug.");
						}
						return true;
					});
				}
			}
			return maxIndex;
		}

		
		private static byte[] SerializeNodeIndices(NavGraph[] graphs)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter wr = new BinaryWriter(memoryStream);
			int maxNodeIndexInAllGraphs = AstarSerializer.GetMaxNodeIndexInAllGraphs(graphs);
			wr.Write(maxNodeIndexInAllGraphs);
			int maxNodeIndex2 = 0;
			for (int i = 0; i < graphs.Length; i++)
			{
				if (graphs[i] != null)
				{
					graphs[i].GetNodes(delegate(GraphNode node)
					{
						maxNodeIndex2 = Math.Max(node.NodeIndex, maxNodeIndex2);
						wr.Write(node.NodeIndex);
						return true;
					});
				}
			}
			if (maxNodeIndex2 != maxNodeIndexInAllGraphs)
			{
				throw new Exception("Some graphs are not consistent in their GetNodes calls, sequential calls give different results.");
			}
			byte[] result = memoryStream.ToArray();
			AstarSerializer.CloseOrDispose(wr);
			return result;
		}

		
		private static byte[] SerializeGraphExtraInfo(NavGraph graph)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(writer);
			graph.SerializeExtraInfo(ctx);
			byte[] result = memoryStream.ToArray();
			AstarSerializer.CloseOrDispose(writer);
			return result;
		}

		
		private static byte[] SerializeGraphNodeReferences(NavGraph graph)
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(writer);
			graph.GetNodes(delegate(GraphNode node)
			{
				node.SerializeReferences(ctx);
				return true;
			});
			AstarSerializer.CloseOrDispose(writer);
			return memoryStream.ToArray();
		}

		
		public void SerializeExtraInfo()
		{
			if (!this.settings.nodes)
			{
				return;
			}
			if (this.graphs == null)
			{
				throw new InvalidOperationException("Cannot serialize extra info with no serialized graphs (call SerializeGraphs first)");
			}
			byte[] array = AstarSerializer.SerializeNodeIndices(this.graphs);
			this.AddChecksum(array);
			this.zip.AddEntry("graph_references.binary", array);
			for (int i = 0; i < this.graphs.Length; i++)
			{
				if (this.graphs[i] != null)
				{
					array = AstarSerializer.SerializeGraphExtraInfo(this.graphs[i]);
					this.AddChecksum(array);
					this.zip.AddEntry("graph" + i + "_extra.binary", array);
					array = AstarSerializer.SerializeGraphNodeReferences(this.graphs[i]);
					this.AddChecksum(array);
					this.zip.AddEntry("graph" + i + "_references.binary", array);
				}
			}
			array = this.SerializeNodeLinks();
			this.AddChecksum(array);
			this.zip.AddEntry("node_link2.binary", array);
		}

		
		private byte[] SerializeNodeLinks()
		{
			MemoryStream memoryStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(memoryStream);
			GraphSerializationContext ctx = new GraphSerializationContext(writer);
			NodeLink2.SerializeReferences(ctx);
			return memoryStream.ToArray();
		}

		
		public void SerializeEditorSettings(GraphEditorBase[] editors)
		{
			if (editors == null || !this.settings.editorSettings)
			{
				return;
			}
			for (int i = 0; i < editors.Length; i++)
			{
				if (editors[i] == null)
				{
					return;
				}
				StringBuilder stringBuilder = AstarSerializer.GetStringBuilder();
				TinyJsonSerializer.Serialize(editors[i], stringBuilder);
				byte[] bytes = this.encoding.GetBytes(stringBuilder.ToString());
				if (bytes.Length > 2)
				{
					this.AddChecksum(bytes);
					this.zip.AddEntry("graph" + i + "_editor.json", bytes);
				}
			}
		}

		
		public bool OpenDeserialize(byte[] bytes)
		{
			this.zipStream = new MemoryStream();
			this.zipStream.Write(bytes, 0, bytes.Length);
			this.zipStream.Position = 0L;
			try
			{
				this.zip = ZipFile.Read(this.zipStream);
			}
			catch (Exception arg)
			{
				Debug.LogError("Caught exception when loading from zip\n" + arg);
				this.zipStream.Dispose();
				return false;
			}
			if (this.zip.ContainsEntry("meta.json"))
			{
				this.meta = this.DeserializeMeta(this.zip["meta.json"]);
			}
			else
			{
				if (!this.zip.ContainsEntry("meta.binary"))
				{
					throw new Exception("No metadata found in serialized data.");
				}
				this.meta = this.DeserializeBinaryMeta(this.zip["meta.binary"]);
			}
			if (AstarSerializer.FullyDefinedVersion(this.meta.version) > AstarSerializer.FullyDefinedVersion(AstarPath.Version))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from a newer version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version,
					"\nThis is usually fine as the stored data is usually backwards and forwards compatible.\nHowever node data (not settings) can get corrupted between versions (even though I try my best to keep compatibility), so it is recommended to recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n"
				}));
			}
			else if (AstarSerializer.FullyDefinedVersion(this.meta.version) < AstarSerializer.FullyDefinedVersion(AstarPath.Version))
			{
				Debug.LogWarning(string.Concat(new object[]
				{
					"Trying to load data from an older version of the A* Pathfinding Project\nCurrent version: ",
					AstarPath.Version,
					" Data version: ",
					this.meta.version,
					"\nThis is usually fine, it just means you have upgraded to a new version.\nHowever node data (not settings) can get corrupted between versions (even though I try my best to keep compatibility), so it is recommended to recalculate any caches (those for faster startup) and resave any files. Even if it seems to load fine, it might cause subtle bugs.\n"
				}));
			}
			return true;
		}

		
		private static Version FullyDefinedVersion(Version v)
		{
			return new Version(Mathf.Max(v.Major, 0), Mathf.Max(v.Minor, 0), Mathf.Max(v.Build, 0), Mathf.Max(v.Revision, 0));
		}

		
		public void CloseDeserialize()
		{
			this.zipStream.Dispose();
			this.zip.Dispose();
			this.zip = null;
			this.zipStream = null;
		}

		
		private NavGraph DeserializeGraph(int zipIndex, int graphIndex)
		{
			Type graphType = this.meta.GetGraphType(zipIndex);
			if (object.Equals(graphType, null))
			{
				return null;
			}
			NavGraph navGraph = this.data.CreateGraph(graphType);
			navGraph.graphIndex = (uint)graphIndex;
			string text = "graph" + zipIndex + ".json";
			string text2 = "graph" + zipIndex + ".binary";
			if (this.zip.ContainsEntry(text))
			{
				TinyJsonDeserializer.Deserialize(AstarSerializer.GetString(this.zip[text]), graphType, navGraph);
			}
			else
			{
				if (!this.zip.ContainsEntry(text2))
				{
					throw new FileNotFoundException(string.Concat(new object[]
					{
						"Could not find data for graph ",
						zipIndex,
						" in zip. Entry 'graph",
						zipIndex,
						".json' does not exist"
					}));
				}
				BinaryReader binaryReader = AstarSerializer.GetBinaryReader(this.zip[text2]);
				GraphSerializationContext ctx = new GraphSerializationContext(binaryReader, null, navGraph.graphIndex, this.meta);
				navGraph.DeserializeSettingsCompatibility(ctx);
			}
			if (navGraph.guid.ToString() != this.meta.guids[zipIndex])
			{
				throw new Exception(string.Concat(new object[]
				{
					"Guid in graph file not equal to guid defined in meta file. Have you edited the data manually?\n",
					navGraph.guid,
					" != ",
					this.meta.guids[zipIndex]
				}));
			}
			return navGraph;
		}

		
		public NavGraph[] DeserializeGraphs()
		{
			List<NavGraph> list = new List<NavGraph>();
			this.graphIndexInZip = new Dictionary<NavGraph, int>();
			for (int i = 0; i < this.meta.graphs; i++)
			{
				int graphIndex = list.Count + this.graphIndexOffset;
				NavGraph navGraph = this.DeserializeGraph(i, graphIndex);
				if (navGraph != null)
				{
					list.Add(navGraph);
					this.graphIndexInZip[navGraph] = i;
				}
			}
			this.graphs = list.ToArray();
			return this.graphs;
		}

		
		private bool DeserializeExtraInfo(NavGraph graph)
		{
			int num = this.graphIndexInZip[graph];
			ZipEntry zipEntry = this.zip["graph" + num + "_extra.binary"];
			if (zipEntry == null)
			{
				return false;
			}
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(zipEntry);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryReader, null, graph.graphIndex, this.meta);
			graph.DeserializeExtraInfo(ctx);
			return true;
		}

		
		private bool AnyDestroyedNodesInGraphs()
		{
			bool result = false;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				this.graphs[i].GetNodes(delegate(GraphNode node)
				{
					if (node.Destroyed)
					{
						result = true;
					}
					return true;
				});
			}
			return result;
		}

		
		private GraphNode[] DeserializeNodeReferenceMap()
		{
			ZipEntry zipEntry = this.zip["graph_references.binary"];
			if (zipEntry == null)
			{
				throw new Exception("Node references not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			BinaryReader reader = AstarSerializer.GetBinaryReader(zipEntry);
			int num = reader.ReadInt32();
			GraphNode[] int2Node = new GraphNode[num + 1];
			try
			{
				for (int i = 0; i < this.graphs.Length; i++)
				{
					this.graphs[i].GetNodes(delegate(GraphNode node)
					{
						int num2 = reader.ReadInt32();
						int2Node[num2] = node;
						return true;
					});
				}
			}
			catch (Exception innerException)
			{
				throw new Exception("Some graph(s) has thrown an exception during GetNodes, or some graph(s) have deserialized more or fewer nodes than were serialized", innerException);
			}
			if (reader.BaseStream.Position != reader.BaseStream.Length)
			{
				throw new Exception(string.Concat(new object[]
				{
					reader.BaseStream.Length / 4L,
					" nodes were serialized, but only data for ",
					reader.BaseStream.Position / 4L,
					" nodes was found. The data looks corrupt."
				}));
			}
			reader.Close();
			return int2Node;
		}

		
		private void DeserializeNodeReferences(NavGraph graph, GraphNode[] int2Node)
		{
			int num = this.graphIndexInZip[graph];
			ZipEntry zipEntry = this.zip["graph" + num + "_references.binary"];
			if (zipEntry == null)
			{
				throw new Exception("Node references for graph " + num + " not found in the data. Was this loaded from an older version of the A* Pathfinding Project?");
			}
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(zipEntry);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryReader, int2Node, graph.graphIndex, this.meta);
			graph.GetNodes(delegate(GraphNode node)
			{
				node.DeserializeReferences(ctx);
				return true;
			});
		}

		
		public void DeserializeExtraInfo()
		{
			bool flag = false;
			for (int i = 0; i < this.graphs.Length; i++)
			{
				flag |= this.DeserializeExtraInfo(this.graphs[i]);
			}
			if (!flag)
			{
				return;
			}
			if (this.AnyDestroyedNodesInGraphs())
			{
				Debug.LogError("Graph contains destroyed nodes. This is a bug.");
			}
			GraphNode[] int2Node = this.DeserializeNodeReferenceMap();
			for (int j = 0; j < this.graphs.Length; j++)
			{
				this.DeserializeNodeReferences(this.graphs[j], int2Node);
			}
			this.DeserializeNodeLinks(int2Node);
		}

		
		private void DeserializeNodeLinks(GraphNode[] int2Node)
		{
			ZipEntry zipEntry = this.zip["node_link2.binary"];
			if (zipEntry == null)
			{
				return;
			}
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(zipEntry);
			GraphSerializationContext ctx = new GraphSerializationContext(binaryReader, int2Node, 0u, this.meta);
			NodeLink2.DeserializeReferences(ctx);
		}

		
		public void PostDeserialization()
		{
			for (int i = 0; i < this.graphs.Length; i++)
			{
				this.graphs[i].PostDeserialization();
			}
		}

		
		public void DeserializeEditorSettings(GraphEditorBase[] graphEditors)
		{
			if (graphEditors == null)
			{
				return;
			}
			for (int i = 0; i < graphEditors.Length; i++)
			{
				if (graphEditors[i] != null)
				{
					for (int j = 0; j < this.graphs.Length; j++)
					{
						if (graphEditors[i].target == this.graphs[j])
						{
							int num = this.graphIndexInZip[this.graphs[j]];
							ZipEntry zipEntry = this.zip["graph" + num + "_editor.json"];
							if (zipEntry != null)
							{
								TinyJsonDeserializer.Deserialize(AstarSerializer.GetString(zipEntry), graphEditors[i].GetType(), graphEditors[i]);
								break;
							}
						}
					}
				}
			}
		}

		
		private static BinaryReader GetBinaryReader(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.Position = 0L;
			return new BinaryReader(memoryStream);
		}

		
		private static string GetString(ZipEntry entry)
		{
			MemoryStream memoryStream = new MemoryStream();
			entry.Extract(memoryStream);
			memoryStream.Position = 0L;
			StreamReader streamReader = new StreamReader(memoryStream);
			string result = streamReader.ReadToEnd();
			memoryStream.Position = 0L;
			streamReader.Dispose();
			return result;
		}

		
		private GraphMeta DeserializeMeta(ZipEntry entry)
		{
			return TinyJsonDeserializer.Deserialize(AstarSerializer.GetString(entry), typeof(GraphMeta), null) as GraphMeta;
		}

		
		private GraphMeta DeserializeBinaryMeta(ZipEntry entry)
		{
			GraphMeta graphMeta = new GraphMeta();
			BinaryReader binaryReader = AstarSerializer.GetBinaryReader(entry);
			if (binaryReader.ReadString() != "A*")
			{
				throw new Exception("Invalid magic number in saved data");
			}
			int num = binaryReader.ReadInt32();
			int num2 = binaryReader.ReadInt32();
			int num3 = binaryReader.ReadInt32();
			int num4 = binaryReader.ReadInt32();
			if (num < 0)
			{
				graphMeta.version = new Version(0, 0);
			}
			else if (num2 < 0)
			{
				graphMeta.version = new Version(num, 0);
			}
			else if (num3 < 0)
			{
				graphMeta.version = new Version(num, num2);
			}
			else if (num4 < 0)
			{
				graphMeta.version = new Version(num, num2, num3);
			}
			else
			{
				graphMeta.version = new Version(num, num2, num3, num4);
			}
			graphMeta.graphs = binaryReader.ReadInt32();
			graphMeta.guids = new List<string>();
			int num5 = binaryReader.ReadInt32();
			for (int i = 0; i < num5; i++)
			{
				graphMeta.guids.Add(binaryReader.ReadString());
			}
			graphMeta.typeNames = new List<string>();
			num5 = binaryReader.ReadInt32();
			for (int j = 0; j < num5; j++)
			{
				graphMeta.typeNames.Add(binaryReader.ReadString());
			}
			return graphMeta;
		}

		
		public static void SaveToFile(string path, byte[] data)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create))
			{
				fileStream.Write(data, 0, data.Length);
			}
		}

		
		public static byte[] LoadFromFile(string path)
		{
			byte[] result;
			using (FileStream fileStream = new FileStream(path, FileMode.Open))
			{
				byte[] array = new byte[(int)fileStream.Length];
				fileStream.Read(array, 0, (int)fileStream.Length);
				result = array;
			}
			return result;
		}

		
		private AstarData data;

		
		private ZipFile zip;

		
		private MemoryStream zipStream;

		
		private GraphMeta meta;

		
		private SerializeSettings settings;

		
		private NavGraph[] graphs;

		
		private Dictionary<NavGraph, int> graphIndexInZip;

		
		private int graphIndexOffset;

		
		private const string binaryExt = ".binary";

		
		private const string jsonExt = ".json";

		
		private uint checksum = uint.MaxValue;

		
		private UTF8Encoding encoding = new UTF8Encoding();

		
		private static StringBuilder _stringBuilder = new StringBuilder();
	}
}
