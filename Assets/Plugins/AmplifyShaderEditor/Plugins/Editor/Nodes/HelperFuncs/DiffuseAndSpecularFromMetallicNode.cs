

using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( 
#if !WB_LANGUAGE_CHINESE
"Diffuse And Specular From Metallic"
#else
"金属漫射和镜面反射"
#endif
,            /*<!C>*/
#if !WB_LANGUAGE_CHINESE
"Miscellaneous"
#else
"其他"
#endif
/*<C!>*/, 
#if !WB_LANGUAGE_CHINESE
"Gets Diffuse and Specular values from Metallic. Uses DiffuseAndSpecularFromMetallic function from UnityStandardUtils."
#else
"从Metallic获取漫反射和镜面反射值。使用UnityStandardUtils的DiffuseAndSpecularFromMetallic函数。"
#endif
)]
	public class DiffuseAndSpecularFromMetallicNode : ParentNode
	{
		
		private const string FuncFormat = "DiffuseAndSpecularFromMetallic({0},{1},{2},{3})";
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, 
#if !WB_LANGUAGE_CHINESE
"Albedo"
#else
"阿尔伯多"
#endif
);
			AddInputPort( WirePortDataType.FLOAT, false, 
#if !WB_LANGUAGE_CHINESE
"Metallic"
#else
"金属漆"
#endif
);
			AddOutputPort( WirePortDataType.FLOAT3, "Out" );
			AddOutputPort( WirePortDataType.FLOAT3, "Spec Color" );
			AddOutputPort( WirePortDataType.FLOAT, "One Minus Reflectivity" );
			m_previewShaderGUID = "c7c4485750948a045b5dab0985896e17";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( dataCollector.IsSRP )
			{
				UIUtils.ShowMessage( UniqueId, "Diffuse And Specular From Metallic Node not compatible with SRP" );
				return m_outputPorts[0].ErrorValue;
			}

			if( m_outputPorts[ outputId ].IsLocalValue( dataCollector.PortCategory ) )
			{
				return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
			}

			dataCollector.AddToIncludes( UniqueId, Constants.UnityStandardUtilsLibFuncs );

			string albedo = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string metallic = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );

			string specColorVar = "specColor" + OutputId;
			string oneMinusReflectivityVar = "oneMinusReflectivity" + OutputId;
			string varName = "diffuseAndSpecularFromMetallic" + OutputId;

			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT3, specColorVar, "(0).xxx" );
			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT, oneMinusReflectivityVar, "0" );


			string varValue = string.Format( FuncFormat, albedo, metallic, specColorVar, oneMinusReflectivityVar );
			dataCollector.AddLocalVariable( UniqueId, PrecisionType.Half, WirePortDataType.FLOAT3, varName, varValue );
			m_outputPorts[ 0 ].SetLocalValue( varName, dataCollector.PortCategory );
			m_outputPorts[ 1 ].SetLocalValue( specColorVar, dataCollector.PortCategory );
			m_outputPorts[ 2 ].SetLocalValue( oneMinusReflectivityVar, dataCollector.PortCategory );

			return m_outputPorts[ outputId ].LocalValue( dataCollector.PortCategory );
		}
	}
}
