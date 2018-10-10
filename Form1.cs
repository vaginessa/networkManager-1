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
        public Form1()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            GetRequestMessage request = new GetRequestMessage(Messenger.NextRequestId,
                VersionCode.V2,
                new OctetString("public"),
                new List<Variable> { new Variable(new ObjectIdentifier("1.3.6.1.2.1.2.1.0")) });//1.3.6.1.2.1.1.1.0")) });

            ISnmpMessage reply = request.GetResponse(60000, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 161));
            if (reply.Pdu().ErrorStatus.ToInt32() != 0) // != ErrorCode.NoError
            {
                throw ErrorException.Create(
                    "error in response",
                    IPAddress.Parse("127.0.0.1"),
                    reply);
            }

            TBgetTest.Text = reply.Pdu().Variables[0].ToString();
        }
    }
}
