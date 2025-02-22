


using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( 
#if !WB_LANGUAGE_CHINESE
"Reflect"
#else
"反思"
#endif
,            /*<!C>*/
#if !WB_LANGUAGE_CHINESE
"Vector Operators"
#else
"矢量运算符"
#endif
/*<C!>*/, 
#if !WB_LANGUAGE_CHINESE
"Reflection vector given an incidence vector and a normal vector"
#else
"给定入射向量和法向量的反射向量"
#endif
, tags: 
#if !WB_LANGUAGE_CHINESE
"refl reflect reflection"
#else
"反射反射"
#endif
)]
	public sealed class ReflectOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].ChangeProperties( "Incident", WirePortDataType.FLOAT4, false );
			m_inputPorts[ 1 ].ChangeProperties( "Normal", WirePortDataType.FLOAT4, false );
			m_outputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );

			m_textLabelWidth = 67;
			m_previewShaderGUID = "fb520f2145c0fa0409320a9e6d720758";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string result = "reflect( " + m_inputA + " , " + m_inputB + " )";
			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}
	}
}
