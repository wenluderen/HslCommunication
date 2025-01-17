﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HslCommunication.Profinet;
using System.Threading;
using HslCommunication.Profinet.Vigor;
using HslCommunication;
using System.Xml.Linq;

namespace HslCommunicationDemo
{
	public partial class FormVigorSerialOverTcp : HslFormContent
	{
		public FormVigorSerialOverTcp( )
		{
			InitializeComponent( );
			vigor = new VigorSerialOverTcp( );
		}


		private VigorSerialOverTcp vigor = null;

		private void FormSiemens_Load( object sender, EventArgs e )
		{
			panel2.Enabled = false;

			Language( Program.Language );
		}


		private void Language( int language )
		{
			if (language == 2)
			{
				Text = "Vigor Read PLC Demo";

				label27.Text = "Ip:";
				label26.Text = "Port:";
				button1.Text = "Connect";
				button2.Text = "Disconnect";
				label21.Text = "Address:";

				label11.Text = "Address:";
				label12.Text = "length:";
				button25.Text = "Bulk Read";
				label13.Text = "Results:";
				label16.Text = "Message:";
				label14.Text = "Results:";
				button26.Text = "Read";

				groupBox3.Text = "Bulk Read test";
				groupBox4.Text = "Message reading test, hex string needs to be filled in";
				groupBox5.Text = "Special function test";
			}
		}

		private void FormSiemens_FormClosing( object sender, FormClosingEventArgs e )
		{

		}
		
		#region Connect And Close

		private void button1_Click( object sender, EventArgs e )
		{
			if (!int.TryParse( textBox2.Text, out int port ))
			{
				MessageBox.Show( DemoUtils.PortInputWrong );
				return;
			}

			vigor?.ConnectClose( );
			vigor = new VigorSerialOverTcp( );
			vigor.IpAddress = textBox1.Text;
			vigor.Port = port;

			try
			{
				vigor.Station = byte.Parse( textBox15.Text );
				OperateResult connect = vigor.ConnectServer( );
				if (connect.IsSuccess)
				{
					MessageBox.Show( HslCommunication.StringResources.Language.ConnectedSuccess );

					button2.Enabled = true;
					button1.Enabled = false;
					panel2.Enabled = true;

					userControlReadWriteOp1.SetReadWriteNet( vigor, "D100", true );
				}
				else
				{
					MessageBox.Show( HslCommunication.StringResources.Language.ConnectedFailed );
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show( ex.Message );
			}
		}

		private void button2_Click( object sender, EventArgs e )
		{
			// 断开连接
			vigor.ConnectClose( );
			button2.Enabled = false;
			button1.Enabled = true;
			panel2.Enabled = false;
		}



		#endregion

		#region 批量读取测试

		private void button25_Click( object sender, EventArgs e )
		{
			DemoUtils.BulkReadRenderResult( vigor, textBox6, textBox9, textBox10 );
		}



		#endregion

		#region 报文读取测试


		private void button26_Click( object sender, EventArgs e )
		{
			OperateResult<byte[]> read = vigor.ReadFromCoreServer( HslCommunication.BasicFramework.SoftBasic.HexStringToBytes( textBox13.Text ) );
			if (read.IsSuccess)
			{
				textBox11.Text = "Result：" + HslCommunication.BasicFramework.SoftBasic.ByteToHexString( read.Content );
			}
			else
			{
				MessageBox.Show( "Read Failed：" + read.ToMessageShowString( ) );
			}
		}


		#endregion
		
		public override void SaveXmlParameter( XElement element )
		{
			element.SetAttributeValue( DemoDeviceList.XmlIpAddress, textBox1.Text );
			element.SetAttributeValue( DemoDeviceList.XmlPort, textBox2.Text );
			element.SetAttributeValue( DemoDeviceList.XmlStation, textBox15.Text );
		}

		public override void LoadXmlParameter( XElement element )
		{
			base.LoadXmlParameter( element );
			textBox1.Text = element.Attribute( DemoDeviceList.XmlIpAddress ).Value;
			textBox2.Text = element.Attribute( DemoDeviceList.XmlPort ).Value;
			textBox15.Text = element.Attribute( DemoDeviceList.XmlStation ).Value;
		}

		private void userControlHead1_SaveConnectEvent_1( object sender, EventArgs e )
		{
			userControlHead1_SaveConnectEvent( sender, e );
		}
	}
}
