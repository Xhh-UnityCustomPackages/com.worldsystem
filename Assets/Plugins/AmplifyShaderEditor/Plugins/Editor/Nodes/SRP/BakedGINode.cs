


using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( 
#if !WB_LANGUAGE_CHINESE
"SRP Baked GI"
#else
"SRP烤GI"
#endif
,            /*<!C>*/
#if !WB_LANGUAGE_CHINESE
"Miscellaneous"
#else
"其他"
#endif
/*<C!>*/, 
#if !WB_LANGUAGE_CHINESE
"Gets Baked GI info."
#else
"获取烘焙GI信息。"
#endif
)]
	public sealed class BakedGINode : ParentNode
	{
		private const string HDBakedGIHeader = "ASEBakedGI( {0}, {1}, {2}, {3} )";
		private readonly string[] HDBakedGIBody =
		{
			"float3 ASEBakedGI( float3 positionWS, float3 normalWS, float2 uvStaticLightmap, float2 uvDynamicLightmap )\n",
			"{\n",
			"\tfloat3 positionRWS = GetCameraRelativePositionWS( positionWS );\n",
			"\treturn SampleBakedGI( positionRWS, normalWS, uvStaticLightmap, uvDynamicLightmap );\n",
			"}\n"
		};

		private readonly string LWBakedGIHeader = "ASEBakedGI( {0}, {1}, {2})";
		private readonly string[] LWBakedGIBody =
		{
		"float3 ASEBakedGI( float3 normalWS, float2 uvStaticLightmap, bool applyScaling )\n",
		"{\n",
		"#ifdef LIGHTMAP_ON\n",
		"\tif( applyScaling )\n",
		"\t\tuvStaticLightmap = uvStaticLightmap * unity_LightmapST.xy + unity_LightmapST.zw;\n",
		"\treturn SampleLightmap( uvStaticLightmap, normalWS );\n",
		"#else\n",
		"\treturn SampleSH(normalWS);\n",
		"#endif\n",
		"}\n"
		};

		private const string ApplyScalingStr = 
#if !WB_LANGUAGE_CHINESE
"Apply Scaling"
#else
"应用缩放"
#endif
;

		[SerializeField]
		private bool m_applyScaling = true;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT3, false, 
#if !WB_LANGUAGE_CHINESE
"World Position"
#else
"世界地位"
#endif
);
			AddInputPort( WirePortDataType.FLOAT3, false, 
#if !WB_LANGUAGE_CHINESE
"World Normal"
#else
"世界正常"
#endif
);
			AddInputPort( WirePortDataType.FLOAT2, false, 
#if !WB_LANGUAGE_CHINESE
"Static UV"
#else
"静态紫外线"
#endif
);
			AddInputPort( WirePortDataType.FLOAT2, false, 
#if !WB_LANGUAGE_CHINESE
"Dynamic UV"
#else
"动态UV"
#endif
);
			AddOutputPort( WirePortDataType.FLOAT3, Constants.EmptyPortValue );
			m_textLabelWidth = 95;
			m_autoWrapProperties = true;
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_applyScaling = EditorGUILayoutToggle( ApplyScalingStr, m_applyScaling );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( !dataCollector.IsSRP )
			{
				UIUtils.ShowMessage( "Node only intended to use on HD and Lightweight rendering pipelines" );
				return GenerateErrorValue();
			}

			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string positionWS = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string normalWS = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string uvStaticLightmap = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			string uvDynamicLightmap = m_inputPorts[ 3 ].GeneratePortInstructions( ref dataCollector );
			string localVarName = "bakedGI" + OutputId;

			if( dataCollector.TemplateDataCollectorInstance.IsHDRP )
			{
				dataCollector.AddFunction( HDBakedGIBody[ 0 ], HDBakedGIBody, false );
				RegisterLocalVariable( 0, string.Format( HDBakedGIHeader, positionWS, normalWS, uvStaticLightmap, uvDynamicLightmap ), ref dataCollector, localVarName );
			}
			else
			{
				dataCollector.AddFunction( LWBakedGIBody[ 0 ], LWBakedGIBody, false );
				RegisterLocalVariable( 0, string.Format( LWBakedGIHeader, normalWS, uvStaticLightmap, m_applyScaling?"true":"false" ), ref dataCollector, localVarName );
			}
			return localVarName;
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			m_applyScaling = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_applyScaling );
		}


	}
}

