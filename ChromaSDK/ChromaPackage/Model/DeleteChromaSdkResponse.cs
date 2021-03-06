﻿using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace ChromaSDK.ChromaPackage.Model
{
	
	[DataContract]
	public class DeleteChromaSdkResponse : IEquatable<DeleteChromaSdkResponse>
	{
		
		public DeleteChromaSdkResponse(int? Result = null)
		{
			this.Result = Result;
		}

		
		
		
		[DataMember(Name = "result")]
		[JsonProperty(PropertyName = "result")]
		public int? Result { get; set; }

		
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class DeleteChromaSdkResponse {\n");
			stringBuilder.Append("  Result: ").Append(this.Result).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		
		public string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		
		public override bool Equals(object obj)
		{
			return this.Equals(obj as DeleteChromaSdkResponse);
		}

		
		public bool Equals(DeleteChromaSdkResponse other)
		{
			if (other == null)
			{
				return false;
			}
			int? result = this.Result;
			int valueOrDefault = result.GetValueOrDefault();
			int? result2 = other.Result;
			return (valueOrDefault == result2.GetValueOrDefault() && result != null == (result2 != null)) || (this.Result != null && this.Result.Equals(other.Result));
		}

		
		public override int GetHashCode()
		{
			int num = 41;
			if (this.Result != null)
			{
				num = num * 59 + this.Result.GetHashCode();
			}
			return num;
		}
	}
}
