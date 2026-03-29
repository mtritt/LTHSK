using System;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HSK
{
    public partial class Form1 : Form
    {
        SqlConnection conn = new SqlConnection(@"Data Source=.;Initial Catalog=QuanLyCuaHangQuanAo;User ID=sa;Password=12345678;TrustServerCertificate=True");

        public Form1()
        {
            InitializeComponent();
            // Bắt và bỏ qua thông báo lỗi mặc định của DataGridView
            //private void dgvSanPham_DataError(object sender, DataGridViewDataErrorEventArgs e)
            //{
            //    e.Cancel = true;
            //}
        }
        private void TinhTongTien()
        {
            decimal tongTien = 0;
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (!row.IsNewRow && row.Cells[5].Value != null)
                {
                    decimal thanhTien;
                    if (decimal.TryParse(row.Cells[5].Value.ToString(), out thanhTien))
                    {
                        tongTien += thanhTien;
                    }
                }
            }
            txtTongTien.Text = tongTien.ToString("N0");
        }

        private bool KiemTraHoaDonTonTai(string maHD, SqlTransaction trans = null)
        {
            bool tonTai = false;
            string query = "SELECT COUNT(*) FROM tblHoaDonBan WHERE MaHDB = @MaHDB";

            // Nếu có transaction đang chạy thì phải gắn vào command
            SqlCommand cmd = new SqlCommand(query, conn);
            if (trans != null) cmd.Transaction = trans;

            cmd.Parameters.AddWithValue("@MaHDB", maHD);

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                int count = (int)cmd.ExecuteScalar();
                if (count > 0) tonTai = true;
            }
            catch { tonTai = false; }

            return tonTai;
        }

        private void LoadThongTinHoaDon(string maHDB)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // A. Tải thông tin chung (Mã KH, Mã NV, Ngày Bán)
                string queryHD = "SELECT * FROM tblHoaDonBan WHERE MaHDB = @MaHDB";
                SqlCommand cmdHD = new SqlCommand(queryHD, conn);
                cmdHD.Parameters.AddWithValue("@MaHDB", maHDB);
                SqlDataReader readerHD = cmdHD.ExecuteReader();
                if (readerHD.Read())
                {
                    txtNhanVien.Text = readerHD["MaNV"].ToString();
                    cboKhachHang.SelectedValue = readerHD["MaKH"];
                    dtpNgay.Value = Convert.ToDateTime(readerHD["NgayBan"]);
                }
                readerHD.Close();

                // B. Tải chi tiết sản phẩm (Móc nối tblChiTietHDB -> tblSanPhamChiTiet -> tblSanPham)
                dgvSanPham.Rows.Clear();
                string queryCT = @"
                    SELECT 
                        sp.MaSP,          -- Để gán vào cột 0 (Sản phẩm)
                        spct.MaSize,      -- Để gán vào cột 1 (Size)
                        spct.MaMau,       -- Để gán vào cột 2 (Màu)
                        ct.SoLuongBan,    -- Để gán vào cột 3 (SL)
                        ct.GiaBan,        -- Để gán vào cột 4 (Giá)
                        ct.ThanhTien      -- Để gán vào cột 5 (Thành Tiền)
                    FROM tblChiTietHDB ct
                    INNER JOIN tblSanPhamChiTiet spct ON ct.MaSPCT = spct.MaSPCT
                    INNER JOIN tblSanPham sp ON spct.MaSP = sp.MaSP
                    WHERE ct.MaHDB = @MaHDB";

                SqlCommand cmdCT = new SqlCommand(queryCT, conn);
                cmdCT.Parameters.AddWithValue("@MaHDB", maHDB);
                SqlDataReader readerCT = cmdCT.ExecuteReader();
                while (readerCT.Read())
                {
                    int rowIndex = dgvSanPham.Rows.Add();
                    DataGridViewRow row = dgvSanPham.Rows[rowIndex];

                    // Gán các giá trị móc nối được vào đúng các cột trong ảnh của bạn
                    row.Cells[0].Value = readerCT["MaSP"].ToString().Trim();       // Đối chiếu hiển thị TenSP (qua ComboBox)
                    row.Cells[1].Value = readerCT["MaSize"].ToString().Trim();     // Size
                    row.Cells[2].Value = readerCT["MaMau"].ToString().Trim();      // Màu
                    row.Cells[3].Value = readerCT["SoLuongBan"].ToString().Trim(); // SL
                    row.Cells[4].Value = Convert.ToDecimal(readerCT["GiaBan"]).ToString("G").Trim(); // Giá
                    row.Cells[5].Value = Convert.ToDecimal(readerCT["ThanhTien"]).ToString("G").Trim(); // Thành tiền
                }
                readerCT.Close();

                TinhTongTien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải chi tiết hóa đơn: " + ex.Message);
            }
        }

        // ==========================================
        // SỰ KIỆN FORM VÀ CONTROL
        // ==========================================\

        private void Form1_Load(object sender, EventArgs e)
        {
            dtpNgay.Value = DateTime.Now;
            txtNhanVien.Text = ""; // Để trống mã NV theo yêu cầu
            //dgvSanPham.DataError += dgvSanPham_DataError;

            dgvSanPham.CellValueChanged -= dgvSanPham_CellValueChanged;
            dgvSanPham.CellValueChanged += dgvSanPham_CellValueChanged;
            dgvSanPham.RowsRemoved -= dgvSanPham_RowsRemoved;
            dgvSanPham.RowsRemoved += dgvSanPham_RowsRemoved;
            dgvSanPham.DataError -= dgvSanPham_DataError;
            dgvSanPham.DataError += dgvSanPham_DataError;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // ========================================================
                // 1. SỬA LỖI TRỐNG COMBOBOX KHÁCH HÀNG
                // ========================================================
                SqlDataAdapter da = new SqlDataAdapter("SELECT MaKH, TenKH FROM tblKhachHang", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // 4 dòng này đã bị thiếu trong code của bạn, mình đã bổ sung lại:
                cboKhachHang.DataSource = dt;
                cboKhachHang.DisplayMember = "MaKH";
                cboKhachHang.ValueMember = "MaKH";
                cboKhachHang.SelectedIndex = -1;

                // ========================================================
                // 2. ĐỔ DỮ LIỆU SẴN CHO CÁC CỘT COMBOBOX TRÊN LƯỚI
                // ========================================================
                // Cột 0: Sản Phẩm (Dùng LTRIM và RTRIM để gọt sạch khoảng trắng 2 đầu)
                SqlDataAdapter daSP = new SqlDataAdapter("SELECT LTRIM(RTRIM(MaSP)) AS MaSP, LTRIM(RTRIM(TenSP)) AS TenSP FROM tblSanPham", conn);
                DataTable dtSP = new DataTable();
                daSP.Fill(dtSP);
                DataGridViewComboBoxColumn colSP = (DataGridViewComboBoxColumn)dgvSanPham.Columns[0];
                colSP.DataSource = dtSP;
                colSP.DisplayMember = "TenSP";
                colSP.ValueMember = "MaSP";

                // Cột 1: Size
                SqlDataAdapter daSize = new SqlDataAdapter("SELECT DISTINCT LTRIM(RTRIM(MaSize)) AS MaSize FROM tblSanPhamChiTiet", conn);
                DataTable dtSize = new DataTable();
                daSize.Fill(dtSize);
                DataGridViewComboBoxColumn colSize = (DataGridViewComboBoxColumn)dgvSanPham.Columns[1];
                colSize.DataSource = dtSize;
                colSize.DisplayMember = "MaSize";
                colSize.ValueMember = "MaSize";

                // Cột 2: Màu
                SqlDataAdapter daMau = new SqlDataAdapter("SELECT DISTINCT LTRIM(RTRIM(MaMau)) AS MaMau FROM tblSanPhamChiTiet", conn);
                DataTable dtMau = new DataTable();
                daMau.Fill(dtMau);
                DataGridViewComboBoxColumn colMau = (DataGridViewComboBoxColumn)dgvSanPham.Columns[2];
                colMau.DataSource = dtMau;
                colMau.DisplayMember = "MaMau";
                colMau.ValueMember = "MaMau";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu khởi tạo: " + ex.Message);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }

        // Tự động tính Thành tiền và Tổng tiền khi sửa Số lượng (Cột 3) hoặc Giá (Cột 4)
        private void dgvSanPham_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == 3 || e.ColumnIndex == 4))
            {
                DataGridViewRow row = dgvSanPham.Rows[e.RowIndex];
                decimal soLuong = 0, gia = 0;

                if (row.Cells[3].Value != null) decimal.TryParse(row.Cells[3].Value.ToString(), out soLuong);
                if (row.Cells[4].Value != null) decimal.TryParse(row.Cells[4].Value.ToString(), out gia);

                row.Cells[5].Value = soLuong * gia;
                TinhTongTien();
            }
        }

        // Tự động trừ tiền đi khi xóa 1 dòng sản phẩm
        private void dgvSanPham_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            TinhTongTien();
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            dgvSanPham.Rows.Add();
        }

        // NÚT THANH TOÁN (Lưu đồng bộ SQL)
        // 2. CẬP NHẬT LẠI NÚT THANH TOÁN (button3_Click)
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHD.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã HĐ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // =========================================================
            // CƠ CHẾ MỚI: KIỂM TRA HÓA ĐƠN TRỐNG (THÀNH TIỀN = 0)
            // =========================================================
            bool isEmpty = true;
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value != null)
                {
                    isEmpty = false; // Có ít nhất 1 sản phẩm hợp lệ
                    break;
                }
            }

            if (isEmpty)
            {
                DialogResult dr = MessageBox.Show("Hóa đơn hiện tại không có sản phẩm. Bạn có muốn XÓA hóa đơn này khỏi hệ thống không?",
                                                  "Xác nhận xóa hóa đơn", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed) conn.Open();
                        using (SqlTransaction trans = conn.BeginTransaction())
                        {
                            try
                            {
                                // Xóa chi tiết trước
                                SqlCommand cmdDelCT = new SqlCommand("DELETE FROM tblChiTietHDB WHERE MaHDB = @MaHDB", conn, trans);
                                cmdDelCT.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                                cmdDelCT.ExecuteNonQuery();

                                // Xóa hóa đơn gốc
                                SqlCommand cmdDelHD = new SqlCommand("DELETE FROM tblHoaDonBan WHERE MaHDB = @MaHDB", conn, trans);
                                cmdDelHD.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                                cmdDelHD.ExecuteNonQuery();

                                trans.Commit();
                                MessageBox.Show("Đã xóa hóa đơn trống khỏi hệ thống!", "Thông báo");
                                btnHuy_Click(sender, e); // Reset form
                            }
                            catch { trans.Rollback(); throw; }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi khi xóa hóa đơn trống: " + ex.Message); }
                    finally { conn.Close(); }
                }
                return; // Dừng hàm tại đây, không thực hiện các bước lưu dưới nữa
            }

            // =========================================================
            // PHẦN LƯU DỮ LIỆU BÌNH THƯỜNG (NẾU CÓ SẢN PHẨM)
            // =========================================================
            DialogResult xacNhanThanhToan = MessageBox.Show("Xác nhận lưu thay đổi hóa đơn?", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhanThanhToan == DialogResult.No) return;

            SqlTransaction transaction = null;
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                transaction = conn.BeginTransaction();

                // 1. Xử lý bảng tblHoaDonBan
                if (KiemTraHoaDonTonTai(txtMaHD.Text.Trim(), transaction))
                {
                    string update = "UPDATE tblHoaDonBan SET MaKH=@MaKH, MaNV=@MaNV, NgayBan=@NgayBan, TongTien=@TongTien WHERE MaHDB=@MaHDB";
                    SqlCommand cmd = new SqlCommand(update, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    decimal.TryParse(txtTongTien.Text, out decimal tt);
                    cmd.Parameters.AddWithValue("@TongTien", tt);
                    cmd.ExecuteNonQuery();

                    // Xóa chi tiết cũ để ghi mới
                    SqlCommand cmdDel = new SqlCommand("DELETE FROM tblChiTietHDB WHERE MaHDB=@MaHDB", conn, transaction);
                    cmdDel.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmdDel.ExecuteNonQuery();
                }
                else
                {
                    string insert = "INSERT INTO tblHoaDonBan(MaHDB, MaKH, MaNV, NgayBan, TongTien) VALUES(@MaHDB, @MaKH, @MaNV, @NgayBan, @TongTien)";
                    SqlCommand cmd = new SqlCommand(insert, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    decimal.TryParse(txtTongTien.Text, out decimal tt);
                    cmd.Parameters.AddWithValue("@TongTien", tt);
                    cmd.ExecuteNonQuery();
                }

                // 2. Ghi chi tiết mới vào tblChiTietHDB
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (!row.IsNewRow && row.Cells[0].Value != null)
                    {
                        // Lấy MaSPCT dựa trên MaSP, Size, Màu (Dùng LIKE để tránh lỗi khoảng trắng)
                        string qSPCT = "SELECT TOP 1 MaSPCT FROM tblSanPhamChiTiet WHERE MaSP LIKE @MaSP AND MaSize LIKE @Size AND MaMau LIKE @Mau";
                        SqlCommand cmdFind = new SqlCommand(qSPCT, conn, transaction);
                        cmdFind.Parameters.AddWithValue("@MaSP", "%" + row.Cells[0].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Size", "%" + row.Cells[1].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Mau", "%" + row.Cells[2].Value.ToString().Trim() + "%");

                        string maSPCT = cmdFind.ExecuteScalar()?.ToString();
                        if (!string.IsNullOrEmpty(maSPCT))
                        {
                            // Chú ý: Bảng tblChiTietHDB có cột ThanhTien tự tính nên không Insert vào đó
                            string insCT = "INSERT INTO tblChiTietHDB(MaHDB, MaSPCT, GiaBan, SoLuongBan) VALUES(@MaHDB, @MaSPCT, @Gia, @SL)";
                            SqlCommand cmdIns = new SqlCommand(insCT, conn, transaction);
                            cmdIns.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                            cmdIns.Parameters.AddWithValue("@MaSPCT", maSPCT);
                            cmdIns.Parameters.AddWithValue("@Gia", Convert.ToDecimal(row.Cells[4].Value));
                            cmdIns.Parameters.AddWithValue("@SL", Convert.ToInt32(row.Cells[3].Value));
                            cmdIns.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                MessageBox.Show("Thanh toán và lưu dữ liệu thành công!", "Thông báo");
                btnHuy_Click(sender, e);
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                MessageBox.Show("Lỗi lưu hóa đơn: " + ex.Message);
            }
            finally { conn.Close(); }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            txtMaHD.Clear();
            cboKhachHang.SelectedIndex = -1;
            dgvSanPham.Rows.Clear();
            txtTongTien.Clear();
            dtpNgay.Value = DateTime.Now;
            txtMaHD.Focus();
        }
        private void dgvSanPham_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Bắt buộc phải có dòng này để chặn tuyệt đối cái bảng đỏ hiện lên
            e.ThrowException = false;
            e.Cancel = true;
        }
        private void btnTimKiem_click_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaHD.Text)) return;

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // 1. Kiểm tra sự tồn tại
                string queryCheck = "SELECT COUNT(*) FROM tblHoaDonBan WHERE LTRIM(RTRIM(MaHDB)) = @MaHDB";
                SqlCommand cmdCheck = new SqlCommand(queryCheck, conn);
                cmdCheck.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                int count = (int)cmdCheck.ExecuteScalar();

                if (count > 0)
                {
                    // 2. Load thông tin chung
                    LoadThongTinHoaDon(txtMaHD.Text.Trim());

                    // 3. Load chi tiết sản phẩm (Đã sửa đúng tên bảng tblChiTietHDB)
                    string queryDetails = @"SELECT sp.MaSP, spct.MaSize, spct.MaMau, ct.SoLuongBan, ct.GiaBan, ct.ThanhTien 
                                    FROM tblChiTietHDB ct
                                    JOIN tblSanPhamChiTiet spct ON ct.MaSPCT = spct.MaSPCT
                                    JOIN tblSanPham sp ON spct.MaSP = sp.MaSP
                                    WHERE LTRIM(RTRIM(ct.MaHDB)) = @MaHDB";

                    SqlDataAdapter da = new SqlDataAdapter(queryDetails, conn);
                    da.SelectCommand.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvSanPham.Rows.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        dgvSanPham.Rows.Add(row["MaSP"], row["MaSize"], row["MaMau"], row["SoLuongBan"], row["GiaBan"], row["ThanhTien"]);
                    }
                    MessageBox.Show("Tìm thấy dữ liệu thành công!", "Thông báo");
                }
                else
                {
                    MessageBox.Show("Mã hóa đơn này chưa tồn tại!", "Thông báo");
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); }
            finally { conn.Close(); }
        }
        private void xacnhan_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.Rows.Count == 0 || (dgvSanPham.Rows.Count == 1 && dgvSanPham.Rows[0].IsNewRow))
            {
                MessageBox.Show("Vui lòng nhập sản phẩm vào lưới trước khi xác nhận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult xacNhan = MessageBox.Show("Bạn chắc chắn muốn thêm sản phẩm chứ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    foreach (DataGridViewRow row in dgvSanPham.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[0].Value != null)
                        {
                            string maSP = row.Cells[0].Value.ToString().Trim();
                            string size = row.Cells[1].Value?.ToString().Trim() ?? "";
                            string mau = row.Cells[2].Value?.ToString().Trim() ?? "";

                            int soLuong = 0;
                            int.TryParse(row.Cells[3].Value?.ToString(), out soLuong);

                            if (string.IsNullOrEmpty(size) || string.IsNullOrEmpty(mau) || soLuong <= 0) continue;

                            string queryGia = "SELECT TOP 1 Giaban FROM tblSanPhamChiTiet WHERE MaSP LIKE '%' + @MaSP + '%' AND MaSize LIKE '%' + @Size + '%' AND MaMau LIKE '%' + @Mau + '%'";

                            SqlCommand cmdGia = new SqlCommand(queryGia, conn);
                            cmdGia.Parameters.AddWithValue("@MaSP", maSP);
                            cmdGia.Parameters.AddWithValue("@Size", size);
                            cmdGia.Parameters.AddWithValue("@Mau", mau);

                            object giaObj = cmdGia.ExecuteScalar();
                            if (giaObj != null && giaObj != DBNull.Value)
                            {
                                decimal giaBan = Convert.ToDecimal(giaObj);
                                row.Cells[4].Value = giaBan;
                                row.Cells[5].Value = giaBan * soLuong;
                            }
                            else
                            {
                                // ========================================================
                                // ĐOẠN CODE "THÁM TỬ" BẮT BỆNH LỖI KẾT NỐI
                                // ========================================================
                                string checkDB = "SELECT COUNT(*) FROM tblSanPhamChiTiet";
                                int totalRows = (int)new SqlCommand(checkDB, conn).ExecuteScalar();

                                string thongBaoLoi = $"C# không tìm thấy giá!\n\n" +
                                                     $"1. Dữ liệu C# đang mang đi tìm:\n" +
                                                     $"   - Mã SP: [{maSP}]\n" +
                                                     $"   - Size: [{size}]\n" +
                                                     $"   - Màu: [{mau}]\n\n" +
                                                     $"2. Tổng số dòng kho hàng C# đang kết nối: {totalRows} dòng.\n" +
                                                     $"(Hãy mở SQL lên đếm xem có đúng {totalRows} dòng không. Nếu khác nhau, bạn đang kết nối sai Database!)";

                                MessageBox.Show(thongBaoLoi, "Chẩn đoán lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    TinhTongTien();
                    // Chỉ báo thành công nếu thực sự có sản phẩm được tính giá
                    MessageBox.Show("Hoàn tất quá trình dò giá!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi xác nhận: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }
        private void xoasp_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng có đang chọn dòng nào không
            if (dgvSanPham.CurrentRow == null || dgvSanPham.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Vui lòng chọn dòng sản phẩm cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hiện Pop-up xác nhận
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn xóa dòng sản phẩm đang chọn không?",
                                              "Xác nhận xóa",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                try
                {
                    // 3. Thực hiện xóa dòng đang được chọn
                    dgvSanPham.Rows.Remove(dgvSanPham.CurrentRow);

                    // 4. QUAN TRỌNG: Cập nhật lại tổng tiền sau khi xóa
                    TinhTongTien();

                    MessageBox.Show("Đã xóa sản phẩm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Không thể xóa dòng này: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
         
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void lblMaHD_Click(object sender, EventArgs e) { }
        private void dgvSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void txtNhanVien_TextChanged(object sender, EventArgs e) { }
        private void txtMaHD_TextChanged(object sender, EventArgs e) { }
        private void txtTongTien_TextChanged(object sender, EventArgs e) { }
        private void dtpNgay_ValueChanged(object sender, EventArgs e) { }
        private void lblNV_Click(object sender, EventArgs e) { }
        private void lblKH_Click(object sender, EventArgs e) { }
        private void cboKhachHang_SelectedIndexChanged(object sender, EventArgs e) { }
        private void lblTongTien_Click(object sender, EventArgs e) { }

      
    }
}