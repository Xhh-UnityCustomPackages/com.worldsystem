


using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( 
#if !WB_LANGUAGE_CHINESE
"Power"
#else
"电源"
#endif
,            /*<!C>*/
#if !WB_LANGUAGE_CHINESE
"Math Operators"
#else
"数学运算符"
#endif
/*<C!>*/, 
#if !WB_LANGUAGE_CHINESE
"Base to the Exp-th power of scalars and vectors"
#else
"基于标量和向量的幂次方"
#endif
, null, KeyCode.E )]
	public sealed class PowerNode : ParentNode
	{
		public const string SafePowerStr = "abs( {0} )";
		public const string SafePowerLabel = 
#if !WB_LANGUAGE_CHINESE
"Safe Power"
#else
"安全电源"
#endif
;

		[SerializeField]
		private bool m_safePower = false;

		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT, false, 
#if !WB_LANGUAGE_CHINESE
"Base"
#else
"基地"
#endif
);
			AddInputPort( WirePortDataType.FLOAT, false, 
#if !WB_LANGUAGE_CHINESE
"Exp"
#else
"费用"
#endif
);
			m_inputPorts[ 1 ].FloatInternalData = 1;
			AddOutputPort( WirePortDataType.FLOAT, Constants.EmptyPortValue );
			m_useInternalPortData = true;
			m_autoWrapProperties = true;
			m_textLabelWidth = 85;
			m_previewShaderGUID = "758cc2f8b537b4e4b93d9833075d138c";
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			UpdateConnections( portId );
		}

		public override void OnConnectedOutputNodeChanges( int inputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
		{
			base.OnConnectedOutputNodeChanges( inputPortId, otherNodeId, otherPortId, name, type );
			UpdateConnections( inputPortId );
		}
		
		void UpdateConnections( int inputPort )
		{
			m_inputPorts[ inputPort ].MatchPortToConnection();
			if ( inputPort == 0 )
			{
				m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
			}
		}

		public override void DrawProperties()
		{
			base.DrawProperties();
			m_safePower = EditorGUILayoutToggle( SafePowerLabel, m_safePower );
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if ( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string x = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			if( m_safePower )
			{
				string safePowerName = "saferPower" + OutputId;
				string safePowerValue = string.Format( SafePowerStr, x );
				dataCollector.AddLocalVariable( UniqueId, CurrentPrecisionType, m_inputPorts[ 0 ].DataType, safePowerName, safePowerValue );
				x = safePowerName;
			}

			string y = m_inputPorts[ 1 ].GenerateShaderForOutput( ref dataCollector, m_inputPorts[ 0 ].DataType, ignoreLocalvar, true );
			string result = "pow( " + x + " , " + y + " )";

			return CreateOutputLocalVariable( 0, result, ref dataCollector );
		}


		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() > 17502 )
				m_safePower = Convert.ToBoolean( GetCurrentParam( ref nodeParams ) );
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
			IOUtils.AddFieldValueToString( ref nodeInfo, m_safePower );
		}
	}
}
