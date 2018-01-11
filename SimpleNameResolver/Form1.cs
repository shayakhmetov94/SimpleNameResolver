using SimpleNameResolver.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleNameResolver
{
    public partial class ResolverForm : Form
    {
        private DnsNameResolver _nameResolver;
        public ResolverForm() {
            InitializeComponent();
            _nameResolver = new DnsNameResolver();
        }

        private void resolveBtn_Click( object sender, EventArgs e ) {//TODO: validate domain name(or truncate)
            if( string.IsNullOrEmpty(domainNameTxtBox.Text) ) {
                MessageBox.Show( this, "Domain name can't be empty" );
                return;
            }

            var ips = _nameResolver.GetHostIpByName(domainNameTxtBox.Text);
            if(ips == null) {
                MessageBox.Show( this, $"Domain {domainNameTxtBox.Text} could not be resolved" );
                return;
            }

            foreach ( var ip in ips )
                ipsListBox.Items.Add( ip );

        }
    }
}
