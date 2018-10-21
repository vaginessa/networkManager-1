using System;
using System.Net;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Security;
using Lextm.SharpSnmpLib.Messaging;
using System.Windows.Forms.DataVisualization.Charting;

namespace networkManager
{
    public partial class Form1 : Form
    {
        const string LOCALHOST = "127.0.0.1";
        const string SERIE = "Valor";
        string[] ids =
{
                // Falhas
                "1.3.6.1.2.1.11.8.0", //snmpInTooBigs
                // Configuração
                "1.3.6.1.2.1.6.9.0", //tcpCurrEstab
                "1.3.6.1.2.1.2.2.1.4.1", //ifMtu
                // Desempenho
                "1.3.6.1.2.1.2.2.1.15.1", //ifInUnknownProtos
                "1.3.6.1.2.1.4.5.0", //IpInAddrErrors
                // Accounting
                "1.3.6.1.2.1.6.5.0", //tcpActiveOpens
                "1.3.6.1.2.1.6.6.0", //tcpPassiveOpens
                // Segurança
                "1.3.6.1.2.1.2.1.0", //snmpInBadCommunityNames

                // Utilização do link
                "1.3.6.1.2.1.2.2.1.5.1", //ifSpeed
                "1.3.6.1.2.1.2.2.1.10.1", //ifInOctets
                "1.3.6.1.2.1.2.2.1.16.1", //ifOutOctets
                
                // Quantidade de pacotes
                "1.3.6.1.2.1.4.10.0", //ipOutRequests - ip enviado
                "1.3.6.1.2.1.4.3.0", //ipInReceives - ip recebido
                "1.3.6.1.2.1.6.10.0", //tcpInSegs - tpc recebidos
                "1.3.6.1.2.1.6.11.0", //tcpOutSegs - tcp enviados
                "1.3.6.1.2.1.7.1.0", //udpInDatagrams - pacotes udp recebidos
                "1.3.6.1.2.1.7.4.0" //udpOutDatagrams - pacotes udp enviados
        };
        public Form1()
        {
            InitializeComponent();
        }

        string Get(string id)
        {
            GetRequestMessage request = new GetRequestMessage(Messenger.NextRequestId,
            VersionCode.V2,
            new OctetString("public"),
            new List<Variable> { new Variable(new ObjectIdentifier(id)) });

            ISnmpMessage reply = request.GetResponse(60000, new IPEndPoint(IPAddress.Parse(LOCALHOST), 161));
            if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(LOCALHOST),
                    reply);
            }
            return reply.Pdu().Variables[0].Data.ToString();
        }

        void Set(string id, int value)
        {
            SetRequestMessage request = new SetRequestMessage(Messenger.NextRequestId,
            VersionCode.V2,
            new OctetString("public"),
            new List<Variable> { new Variable(new ObjectIdentifier(id), new OctetString(value.ToString())) });

            ISnmpMessage reply = request.GetResponse(60000, new IPEndPoint(IPAddress.Parse(LOCALHOST), 161));
            if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(LOCALHOST),
                    reply);
            }
        }

        void Set(string id, string value)
        {
            SetRequestMessage request = new SetRequestMessage(Messenger.NextRequestId,
            VersionCode.V2,
            new OctetString("public"),
            new List<Variable> { new Variable(new ObjectIdentifier(id), new OctetString(value)) });

            ISnmpMessage reply = request.GetResponse(60000, new IPEndPoint(IPAddress.Parse(LOCALHOST), 161));
            if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse(LOCALHOST),
                    reply);
            }
        }

        List<Variable> Walk(string id)
        {
            var result = new List<Variable>();
#pragma warning disable CS0618 // O tipo ou membro é obsoleto
            Messenger.BulkWalk(VersionCode.V2,
                              new IPEndPoint(IPAddress.Parse(LOCALHOST), 161),
                              new OctetString("public"),
                              new ObjectIdentifier(id),
                              result,
                              60000,
                              10,
                              WalkMode.WithinSubtree,
                              null,
                              null);
#pragma warning restore CS0618 // O tipo ou membro é obsoleto
            return result;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] results = new string[ids.Length];

            for (int i = 0; i < ids.Length; i++)
                results[i] = Get(ids[i]);

            int j = 0;
            foreach (TabPage tempPage in tabControl1.Controls.OfType<TabPage>())
            {
                foreach (Chart tempChart in tempPage.Controls.OfType<Chart>())
                {
                    tempChart.Series[SERIE].Points.AddY(results[j]);
                    j++;
                }
            }            
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string result = Get(textBox1.Text).ToString();
            textBox1.Text = result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string temp = comboBox1.Text;
            List<Variable> result = Walk(temp);
            int i = 0;
            comboBox1.Items.Clear();
            while (i < result.Count)
            {
                comboBox1.Items.Add(i + ": " + result[i].Data.ToString());
                i++;
            }
        }
    }
}