using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BTbuoi5
{
    public partial class EditForm : Form
    {
        public string StudentID { get; set; }
        public string StudentName { get; set; }
        public string Faculty { get; set; }
        public string AverageScore { get; set; }
        public EditForm()
        {
            InitializeComponent();
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin sinh viên lên các ô nhập liệu
            txtMa.Text = StudentID;
            txtTen.Text = StudentName;
            cmbKhoa.Text = Faculty;
            txtDiemTB.Text = AverageScore;
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Lưu thông tin đã chỉnh sửa vào các biến
            StudentID = txtMa.Text;
            StudentName = txtTen.Text;
            Faculty = cmbKhoa.Text;
            AverageScore = txtDiemTB.Text;

            // Đóng Form con và trả lại kết quả
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            // Đóng form mà không thực hiện thay đổi
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
