using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;

namespace dnstoolsform
{
    public partial class Form1 : Form
    {

        HttpClient httpClient = new HttpClient();
        public Form1()
        {
            InitializeComponent();


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private async Task<string> getURL(string url) {
            var res = await httpClient.GetAsync(url);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadAsStringAsync();
        }

        public async Task<string[]> dig(string domena, string rekord, string serwerdns="") {
            string res = await getURL($"http://10.40.50.201:4000/dig?domena={domena}&rekord={rekord}&serwerdns={serwerdns}");
            dnsRes odp = JsonSerializer.Deserialize<dnsRes>(res);
            return odp.data.Split("\n");
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            string[] tab = { "A", "NS", "MX" };
            string[] ns = { };

            textBox2.Text = "";
            foreach (string rek in tab)
            {
                textBox2.Text += $"{rek}: \r\n";
                var odp = await dig(textBox1.Text, rek);
                if (rek == "NS")
                    ns = odp;
                foreach (string res in odp)
                {
                    if(res!="")
                            textBox2.Text += $"\t\t{res} \r\n";
                }
            }

            textBox2.Text += "\r\n----------- Badanie serwerów ns ------------\r\n";
            foreach(string servns in ns)
            {
                if (servns != "")
                {
                    string[] res = await dig(textBox1.Text, "NS", servns);
                    if (res.Length > 0)
                        textBox2.Text += $"{servns}:\tOK\r\n";
                    else
                        textBox2.Text += $"{servns}:\tnOK\r\n";
                }
            }

        }
    }
}
