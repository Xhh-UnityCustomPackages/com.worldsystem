


using System;
using UnityEngine;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( 
#if !WB_LANGUAGE_CHINESE
"Dot"
#else
"圆点"
#endif
,            /*<!C>*/
#if !WB_LANGUAGE_CHINESE
"Vector Operators"
#else
"矢量运算符"
#endif
/*<C!>*/, 
#if !WB_LANGUAGE_CHINESE
"Scalar dot product of two vectors ( A . B )"
#else
"两个向量的标量点积（A.B）"
#endif
, null, KeyCode.Period )]
	public sealed class DotProductOpNode : DynamicTypeNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_inputPorts[ 1 ].ChangeType( WirePortDataType.FLOAT4, false );
			m_dynamicOutputType = false;
			m_useInternalPortData = true;
			m_allowMatrixCheck = true;
			m_previewShaderGUID = "85f11fd5cb9bb954c8615a45c57a3784";
		}

		public override string BuildResults( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			base.BuildResults( outputId, ref dataCollector, ignoreLocalvar );
			string result = "dot( " + m_inputA + " , " + m_inputB + " )";
			RegisterLocalVariable( 0, result, ref dataCollector, "dotResult" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}
	}
}
