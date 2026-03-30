using System;
using System.Drawing;
using System.Windows.Forms;

namespace BMI計算機
{
    public partial class frmBMI : Form
    {
        private Button btnClear;
        private Button btnSample;
        private Label lblCategory;
        private Label lblRange;
        private Label lblAdvice;
        private Label lblInputHint;
        private Label lblWaistTip;
        private Label lblFormula;

        // 跑馬燈
        private Panel pnlMarquee;
        private Label lblMarquee;
        private Timer tmrMarquee;

        private ToolTip tip = new ToolTip();

        public frmBMI()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetupBasicUI();
            CreateExtraControls();
            CreateMarqueeArea();
            ResetResult();

            // 鍵盤控制：上下鍵切換輸入框，Enter 計算
            txtHeight.KeyDown += InputBox_KeyDown;
            txtWeight.KeyDown += InputBox_KeyDown;
        }

        private void SetupBasicUI()
        {
            this.Text = "BMI 健康計算機";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AcceptButton = btnRun;
            this.BackColor = Color.FromArgb(245, 248, 252);

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // 預設值
            txtHeight.Text = "170";
            txtWeight.Text = "60";

            // Tab 順序
            txtHeight.TabIndex = 0;
            txtWeight.TabIndex = 1;
            btnRun.TabIndex = 2;

            // 輸入框字型
            txtHeight.Font = new Font("微軟正黑體", 12F, FontStyle.Regular);
            txtWeight.Font = new Font("微軟正黑體", 12F, FontStyle.Regular);

            // 計算按鈕美化
            btnRun.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            btnRun.BackColor = Color.SteelBlue;
            btnRun.ForeColor = Color.White;
            btnRun.FlatStyle = FlatStyle.Flat;
            btnRun.FlatAppearance.BorderSize = 0;

            // 結果框
            lblResult.AutoSize = false;
            lblResult.Size = new Size(150, 42);
            lblResult.TextAlign = ContentAlignment.MiddleCenter;
            lblResult.BorderStyle = BorderStyle.FixedSingle;
            lblResult.Font = new Font("微軟正黑體", 18F, FontStyle.Bold);
            lblResult.BackColor = Color.White;
            lblResult.ForeColor = Color.Black;

            // 提示
            tip.SetToolTip(txtHeight, "請輸入身高（公分），例如：170");
            tip.SetToolTip(txtWeight, "請輸入體重（公斤），例如：60");
            tip.SetToolTip(btnRun, "按一下或按 Enter 計算 BMI");

            // 點文字框自動全選
            txtHeight.Click += TextBox_SelectAll;
            txtWeight.Click += TextBox_SelectAll;
        }

        private void CreateExtraControls()
        {
            Control inputParent = btnRun.Parent;
            Control resultParent = lblResult.Parent;

            // 清除按鈕
            btnClear = new Button();
            btnClear.Name = "btnClear";
            btnClear.Text = "清除";
            btnClear.Size = new Size(78, 34);
            btnClear.Location = new Point(btnRun.Left, btnRun.Bottom + 12);
            btnClear.Font = new Font("微軟正黑體", 9.5F, FontStyle.Bold);
            btnClear.BackColor = Color.Gray;
            btnClear.ForeColor = Color.White;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.FlatAppearance.BorderSize = 0;
            btnClear.TabIndex = 3;
            btnClear.Click += btnClear_Click;
            inputParent.Controls.Add(btnClear);

            // 範例按鈕
            btnSample = new Button();
            btnSample.Name = "btnSample";
            btnSample.Text = "範例";
            btnSample.Size = new Size(78, 34);
            btnSample.Location = new Point(btnClear.Right + 10, btnRun.Bottom + 12);
            btnSample.Font = new Font("微軟正黑體", 9.5F, FontStyle.Bold);
            btnSample.BackColor = Color.SeaGreen;
            btnSample.ForeColor = Color.White;
            btnSample.FlatStyle = FlatStyle.Flat;
            btnSample.FlatAppearance.BorderSize = 0;
            btnSample.TabIndex = 4;
            btnSample.Click += btnSample_Click;
            inputParent.Controls.Add(btnSample);

            this.CancelButton = btnClear;

            tip.SetToolTip(btnClear, "清空輸入與結果");
            tip.SetToolTip(btnSample, "快速帶入範例資料");

            // 輸入區提示
            lblInputHint = new Label();
            lblInputHint.Name = "lblInputHint";
            lblInputHint.AutoSize = false;
            lblInputHint.Size = new Size(500, 28);
            lblInputHint.Location = new Point(40, btnClear.Bottom + 10);
            lblInputHint.Font = new Font("微軟正黑體", 10F, FontStyle.Regular);
            lblInputHint.TextAlign = ContentAlignment.MiddleLeft;
            lblInputHint.ForeColor = Color.FromArgb(40, 40, 40);
            inputParent.Controls.Add(lblInputHint);

            // 腰圍提示
            lblWaistTip = new Label();
            lblWaistTip.Name = "lblWaistTip";
            lblWaistTip.AutoSize = false;
            lblWaistTip.Size = new Size(520, 38);
            lblWaistTip.Location = new Point(40, lblInputHint.Bottom + 2);
            lblWaistTip.Font = new Font("微軟正黑體", 9.5F, FontStyle.Regular);
            lblWaistTip.TextAlign = ContentAlignment.MiddleLeft;
            lblWaistTip.ForeColor = Color.Firebrick;
            lblWaistTip.Text = "腰圍提醒：男性 ≥ 90 cm、女性 ≥ 80 cm，表示腹部肥胖風險較高。";
            inputParent.Controls.Add(lblWaistTip);

            // 體位分類
            lblCategory = new Label();
            lblCategory.Name = "lblCategory";
            lblCategory.AutoSize = false;
            lblCategory.Size = new Size(150, 40);
            lblCategory.Location = new Point(lblResult.Left, lblResult.Bottom + 12);
            lblCategory.Font = new Font("微軟正黑體", 12F, FontStyle.Bold);
            lblCategory.TextAlign = ContentAlignment.MiddleCenter;
            lblCategory.BorderStyle = BorderStyle.FixedSingle;
            lblCategory.BackColor = Color.WhiteSmoke;
            resultParent.Controls.Add(lblCategory);

            // 健康體重範圍
            lblRange = new Label();
            lblRange.Name = "lblRange";
            lblRange.AutoSize = false;
            lblRange.Size = new Size(380, 28);
            lblRange.Location = new Point(lblResult.Left, lblCategory.Bottom + 14);
            lblRange.Font = new Font("微軟正黑體", 10.5F, FontStyle.Regular);
            lblRange.TextAlign = ContentAlignment.MiddleLeft;
            resultParent.Controls.Add(lblRange);

            // 建議文字
            lblAdvice = new Label();
            lblAdvice.Name = "lblAdvice";
            lblAdvice.AutoSize = false;
            lblAdvice.Size = new Size(420, 42);
            lblAdvice.Location = new Point(lblResult.Left, lblRange.Bottom + 8);
            lblAdvice.Font = new Font("微軟正黑體", 10.5F, FontStyle.Regular);
            lblAdvice.TextAlign = ContentAlignment.TopLeft;
            resultParent.Controls.Add(lblAdvice);

            // BMI 公式
            lblFormula = new Label();
            lblFormula.Name = "lblFormula";
            lblFormula.AutoSize = false;
            lblFormula.Size = new Size(300, 38);
            lblFormula.Location = new Point(lblResult.Left, lblAdvice.Bottom + 6);
            lblFormula.Font = new Font("微軟正黑體", 10F, FontStyle.Bold);
            lblFormula.TextAlign = ContentAlignment.MiddleCenter;
            lblFormula.BorderStyle = BorderStyle.FixedSingle;
            lblFormula.BackColor = Color.FromArgb(255, 248, 232);
            lblFormula.ForeColor = Color.FromArgb(50, 50, 50);
            lblFormula.Text = "公式：BMI = 體重(公斤) ÷ 身高²(公尺)";
            resultParent.Controls.Add(lblFormula);
        }

        private void CreateMarqueeArea()
        {
            pnlMarquee = new Panel();
            pnlMarquee.Name = "pnlMarquee";
            pnlMarquee.Size = new Size(280, 60);
            pnlMarquee.Location = new Point(this.ClientSize.Width - 310, 150);
            pnlMarquee.BackColor = Color.White;
            pnlMarquee.BorderStyle = BorderStyle.FixedSingle;
            pnlMarquee.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.Controls.Add(pnlMarquee);

            lblMarquee = new Label();
            lblMarquee.Name = "lblMarquee";
            lblMarquee.AutoSize = true;
            lblMarquee.Font = new Font("微軟正黑體", 16F, FontStyle.Bold);
            lblMarquee.ForeColor = Color.SteelBlue;
            lblMarquee.BackColor = Color.Transparent;
            lblMarquee.Text = "歡迎使用 BMI 健康計算機";
            lblMarquee.Location = new Point(pnlMarquee.Width, 14);
            pnlMarquee.Controls.Add(lblMarquee);

            tmrMarquee = new Timer();
            tmrMarquee.Interval = 40;
            tmrMarquee.Tick += tmrMarquee_Tick;
            tmrMarquee.Start();
        }

        private void tmrMarquee_Tick(object sender, EventArgs e)
        {
            lblMarquee.Left -= 3;

            if (lblMarquee.Right < 0)
            {
                lblMarquee.Left = pnlMarquee.Width;
            }
        }

        private void InputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender == txtHeight)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    txtWeight.Focus();
                    txtWeight.SelectAll();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnRun.PerformClick();
                    e.SuppressKeyPress = true;
                }
            }
            else if (sender == txtWeight)
            {
                if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
                {
                    txtHeight.Focus();
                    txtHeight.SelectAll();
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    btnRun.PerformClick();
                    e.SuppressKeyPress = true;
                }
            }
        }

        private void TextBox_SelectAll(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            if (txt != null)
            {
                txt.SelectAll();
            }
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            bool isHeightValid = double.TryParse(txtHeight.Text, out double height);
            bool isWeightValid = double.TryParse(txtWeight.Text, out double weight);

            string errorMessage = "";

            txtHeight.BackColor = Color.White;
            txtWeight.BackColor = Color.White;

            // 驗證身高
            if (!isHeightValid)
            {
                errorMessage += "身高：請輸入數字（例如 170）\n";
                txtHeight.BackColor = Color.MistyRose;
            }
            else if (height <= 0)
            {
                errorMessage += "身高：必須大於 0\n";
                txtHeight.BackColor = Color.MistyRose;
            }

            // 驗證體重
            if (!isWeightValid)
            {
                errorMessage += "體重：請輸入數字（例如 60）\n";
                txtWeight.BackColor = Color.MistyRose;
            }
            else if (weight <= 0)
            {
                errorMessage += "體重：必須大於 0\n";
                txtWeight.BackColor = Color.MistyRose;
            }

            if (errorMessage != "")
            {
                ShowError("請修正以下輸入：\n" + errorMessage);

                if (txtHeight.BackColor == Color.MistyRose)
                {
                    txtHeight.Focus();
                    txtHeight.SelectAll();
                }
                else
                {
                    txtWeight.Focus();
                    txtWeight.SelectAll();
                }
                return;
            }

            double heightInMeters = height / 100.0;
            double bmi = weight / (heightInMeters * heightInMeters);

            string category = "";
            Color resultColor = Color.White;
            string advice = "";

            if (bmi < 18.5)
            {
                category = "體重過輕";
                resultColor = Color.LightSkyBlue;
                advice = "建議均衡飲食並適度增加營養攝取。";
            }
            else if (bmi < 24)
            {
                category = "健康體位";
                resultColor = Color.LightGreen;
                advice = "目前狀況良好，請繼續保持。";
            }
            else if (bmi < 27)
            {
                category = "體位過重";
                resultColor = Color.Gold;
                advice = "建議增加運動並注意飲食習慣。";
            }
            else if (bmi < 30)
            {
                category = "輕度肥胖";
                resultColor = Color.Orange;
                advice = "建議開始規律運動與體重控制。";
            }
            else if (bmi < 35)
            {
                category = "中度肥胖";
                resultColor = Color.IndianRed;
                advice = "建議積極調整生活習慣並持續監測。";
            }
            else
            {
                category = "重度肥胖";
                resultColor = Color.MediumPurple;
                advice = "建議儘早尋求專業健康管理建議。";
            }

            double minWeight = 18.5 * heightInMeters * heightInMeters;
            double maxWeight = 24.0 * heightInMeters * heightInMeters;
            double idealMid = (minWeight + maxWeight) / 2.0;
            double diff = weight - idealMid;

            string diffText = "";
            if (diff > 0)
            {
                diffText = $"目前約比理想體重中間值高 {diff:F1} kg。";
            }
            else if (diff < 0)
            {
                diffText = $"目前約比理想體重中間值低 {Math.Abs(diff):F1} kg。";
            }
            else
            {
                diffText = "目前非常接近理想體重中間值。";
            }

            lblResult.Text = $"{bmi:F2}";
            lblResult.BackColor = resultColor;
            lblResult.ForeColor = Color.Black;

            lblCategory.Text = category;
            lblCategory.BackColor = resultColor;
            lblCategory.ForeColor = Color.Black;

            lblRange.Text = $"健康體重範圍：約 {minWeight:F1} ~ {maxWeight:F1} kg";
            lblAdvice.Text = $"建議：{advice}\n{diffText}";
        }

        private void ShowError(string message)
        {
            lblResult.Text = "錯誤";
            lblResult.BackColor = Color.MistyRose;
            lblResult.ForeColor = Color.DarkRed;

            lblCategory.Text = "輸入有誤";
            lblCategory.BackColor = Color.MistyRose;
            lblCategory.ForeColor = Color.DarkRed;

            lblRange.Text = "健康體重範圍：-- ~ -- kg";
            lblAdvice.Text = message;
        }

        private void ResetResult()
        {
            txtHeight.BackColor = Color.White;
            txtWeight.BackColor = Color.White;

            lblResult.Text = "--.--";
            lblResult.BackColor = Color.White;
            lblResult.ForeColor = Color.Black;

            if (lblCategory != null)
            {
                lblCategory.Text = "尚未計算";
                lblCategory.BackColor = Color.WhiteSmoke;
                lblCategory.ForeColor = Color.Black;
            }

            if (lblRange != null)
            {
                lblRange.Text = "健康體重範圍：-- ~ -- kg";
            }

            if (lblAdvice != null)
            {
                lblAdvice.Text = "";
            }

            if (lblInputHint != null)
            {
                lblInputHint.Text = "可用 ↑ ↓ 切換身高與體重輸入框，按 Enter 直接計算。";
            }

            if (lblWaistTip != null)
            {
                lblWaistTip.Text = "腰圍提醒：男性 ≥ 90 cm、女性 ≥ 80 cm，表示腹部肥胖風險較高。";
            }

            if (lblFormula != null)
            {
                lblFormula.Text = "公式：BMI = 體重(公斤) ÷ 身高²(公尺)";
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtHeight.Text = "";
            txtWeight.Text = "";
            ResetResult();
            txtHeight.Focus();
        }

        private void btnSample_Click(object sender, EventArgs e)
        {
            txtHeight.Text = "170";
            txtWeight.Text = "60";
            txtHeight.BackColor = Color.White;
            txtWeight.BackColor = Color.White;
            txtHeight.Focus();
            txtHeight.SelectAll();
        }

        private void lblResult_Click(object sender, EventArgs e)
        {
        }

        private void lblBMI_Click(object sender, EventArgs e)
        {
        }
    }
}