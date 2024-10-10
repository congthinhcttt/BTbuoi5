using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BTbuoi5.Thinh;
using BTbuoi5;

namespace BTbuoi5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                using (Model1 model1 = new Model1())
                {
                    List<Faculty> listFaculties = model1.Faculties.ToList();
                    List<Student> listStudents = model1.Students.ToList();
                    FillFacultyCombobox(listFaculties);
                    BindGrid(listStudents);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }


        private void FillFacultyCombobox(List<Faculty> listFaculties)
        {
            cmbKhoa.DataSource = listFaculties;
            cmbKhoa.DisplayMember = "FacultyName";
            cmbKhoa.ValueMember = "FacultyID";
        }

        private void BindGrid(List<Student> listStudent)
        {
            dataGridView1.Rows.Clear();
            foreach (Student student in listStudent)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = student.StudentID;
                dataGridView1.Rows[index].Cells[1].Value = student.StudentName;
                dataGridView1.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                dataGridView1.Rows[index].Cells[3].Value = student.AverageScore;
            }
        }

      

        private void loadDaTa()
        {
            using (Model1 model1 = new Model1())
            {
                List<Student> listStudent = model1.Students.ToList();
                BindGrid(listStudent);
            }
        }

        private void cmbKhoa_SelectedIndexChanged(object sender, EventArgs e)
        {
            string khoa = cmbKhoa.SelectedItem.ToString();
        }

        private void ResetFom()
        {
            txtMa.Clear();
            txtTen.Clear();
            txtDiemTB.Clear();
            cmbKhoa.SelectedIndex = -1;
        }

        private int GetSelectedRow(string StudentID)
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null &&
                    dataGridView1.Rows[i].Cells[0].Value.ToString() == StudentID)
                {
                    return i;
                }

            }
            return -1;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                // Điền thông tin sinh viên vào các ô nhập liệu
                txtMa.Text = row.Cells[0].Value.ToString();
                txtTen.Text = row.Cells[1].Value.ToString();
                cmbKhoa.Text = row.Cells[2].Value.ToString();
                txtDiemTB.Text = row.Cells[3].Value.ToString();
            }
        }

        private void InsertUpdate(int selectedRow)
        {
            dataGridView1.Rows[selectedRow].Cells[0].Value = txtMa.Text;
            dataGridView1.Rows[selectedRow].Cells[1].Value = txtTen.Text;
            dataGridView1.Rows[selectedRow].Cells[2].Value = cmbKhoa.Text;
            dataGridView1.Rows[selectedRow].Cells[3].Value = float.Parse(txtDiemTB.Text).ToString();

        }

       

        private void btnThem_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtDiemTB.Text == "" || txtTen.Text == "" || txtMa.Text == "")
                    throw new Exception("Vui lòng nhập đầy đủ thông tin sinh viên !");

                // Kiểm tra sinh viên đã tồn tại hay chưa
                int selectedRow = GetSelectedRow(txtMa.Text);
                if (selectedRow == -1)
                {
                    using (Model1 model1 = new Model1())
                    {
                        // Thêm sinh viên vào cơ sở dữ liệu
                        var faculty = model1.Faculties.FirstOrDefault(f => f.FacultyName == cmbKhoa.Text);
                        if (faculty == null)
                            throw new Exception("Khoa không tồn tại!");

                        var student = new Student
                        {
                            StudentID = txtMa.Text,
                            StudentName = txtTen.Text,
                            AverageScore = float.Parse(txtDiemTB.Text),
                            Faculty = faculty
                        };

                        model1.Students.Add(student);
                        model1.SaveChanges(); // Lưu thay đổi vào cơ sở dữ liệu
                    }

                    // Thêm vào DataGridView
                    selectedRow = dataGridView1.Rows.Add();
                    InsertUpdate(selectedRow);
                    MessageBox.Show("Thêm dữ liệu thành công", "Thông Báo !", MessageBoxButtons.OK);
                }
                else
                {
                    MessageBox.Show("Sinh viên đã tồn tại!", "Thông Báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnSua_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng có chọn hàng hay chưa
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                // Lấy hàng đang được chọn trong DataGridView
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];

                // Tạo một instance của EditForm và truyền dữ liệu hiện tại vào
                EditForm editForm = new EditForm()
                {
                    StudentID = selectedRow.Cells[0].Value.ToString(),
                    StudentName = selectedRow.Cells[1].Value.ToString(),
                    Faculty = selectedRow.Cells[2].Value.ToString(),
                    AverageScore = selectedRow.Cells[3].Value.ToString()
                };

                // Hiển thị EditForm và kiểm tra xem người dùng có nhấn Lưu hay Hủy
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    // Cập nhật thông tin sinh viên trong DataGridView
                    selectedRow.Cells[0].Value = editForm.StudentID;
                    selectedRow.Cells[1].Value = editForm.StudentName;
                    selectedRow.Cells[2].Value = editForm.Faculty;
                    selectedRow.Cells[3].Value = editForm.AverageScore;

                    // Cập nhật thông tin vào cơ sở dữ liệu
                    using (var model1 = new Model1())
                    {
                        // Tìm sinh viên trong cơ sở dữ liệu
                        var student = model1.Students.FirstOrDefault(s => s.StudentID == editForm.StudentID);
                        if (student != null)
                        {
                            student.StudentName = editForm.StudentName;
                            student.AverageScore = float.Parse(editForm.AverageScore);

                            // Cập nhật thông tin khoa của sinh viên
                            var faculty = model1.Faculties.FirstOrDefault(f => f.FacultyName == editForm.Faculty);
                            if (faculty != null)
                            {
                                student.Faculty = faculty;
                            }

                            // Lưu thay đổi vào cơ sở dữ liệu
                            model1.SaveChanges();

                            MessageBox.Show("Cập nhật thông tin thành công!", "Thông Báo", MessageBoxButtons.OK);
                        }
                    }
                }
                else
                {
                    // Nếu người dùng bấm Hủy, không làm gì cả.
                    MessageBox.Show("Bạn đã hủy thao tác sửa.", "Thông Báo", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK);
            }
        }

       
        private void btnXoa_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra xem người dùng có chọn hàng hay chưa
                if (dataGridView1.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK);
                    return;
                }

                // Lấy hàng đang được chọn trong DataGridView
                DataGridViewRow selectedRow = dataGridView1.SelectedRows[0];
                string studentID = selectedRow.Cells[0].Value.ToString();

                // Hỏi người dùng có chắc chắn muốn xóa không
                DialogResult dialogResult = MessageBox.Show("Bạn có chắc chắn muốn xóa sinh viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    // Xóa sinh viên khỏi cơ sở dữ liệu
                    using (var model1 = new Model1())
                    {
                        var student = model1.Students.FirstOrDefault(s => s.StudentID == studentID);
                        if (student != null)
                        {
                            model1.Students.Remove(student);
                            model1.SaveChanges();  // Lưu thay đổi vào cơ sở dữ liệu để đảm bảo dữ liệu bị xóa vĩnh viễn

                            MessageBox.Show("Xóa sinh viên thành công!", "Thông Báo", MessageBoxButtons.OK);

                            // Xóa sinh viên khỏi DataGridView
                            dataGridView1.Rows.Remove(selectedRow);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sinh viên để xóa.", "Lỗi", MessageBoxButtons.OK);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn Đang muốn thoát", "Thông Báo", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                Close();
            }
        }
    }
}
