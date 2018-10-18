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

namespace networkManager
{
    public partial class Form1 : Form
    {
        const string LOCALHOST = "127.0.0.1";
        public Form1()
        {
            InitializeComponent();
        }

        void Get(string id)
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

            TBgetTest.Text = reply.Pdu().Variables[0].ToString();
        }

        void Set(string id, int value)
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

        void Walk(string id)
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
            int i = 0;
            while (i < result.Count)
            {
                TBgetTest.Text = result[i].ToString();
                i++;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Get("1.3.6.1.2.1.2.1.0");

        }
    }
}
