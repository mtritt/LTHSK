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
        SqlConnection conn = new SqlConnection(@"Data Source=DESKTOP-7BCKKNM;Initial Catalog=QuanLyCuaHangQuanAo;User ID=sa;Password=12345678;TrustServerCertificate=True");
        decimal tongTienCu = 0;
        public Form1()
        {
            InitializeComponent();
        }
        private void TinhTongTien()
        {
            decimal tongTienChuaThanhToan = 0;

            // Kiểm tra trạng thái hiện tại (đưa về chữ thường để so sánh tránh lỗi font/dấu cách)
            bool isDaThanhToan = lblTrangThai.Text.Trim().ToLower() == "đã thanh toán";

            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                // Bỏ qua dòng trống cuối cùng và những dòng chưa có Thành Tiền
                if (!row.IsNewRow && row.Cells[5].Value != null)
                {
                    decimal thanhTienDong = 0;
                    decimal.TryParse(row.Cells[5].Value.ToString(), out thanhTienDong);

                    if (isDaThanhToan)
                    {
                        // NẾU ĐƠN ĐÃ THANH TOÁN: Chỉ cộng tiền những dòng MỚI THÊM
                        // (Những dòng cũ tải từ SQL lên đã được đánh dấu Tag = "Old" trong hàm LoadThongTinHoaDon)
                        if (row.Tag == null || row.Tag.ToString() != "Old")
                        {
                            tongTienChuaThanhToan += thanhTienDong;
                        }
                    }
                    else
                    {
                        // NẾU ĐƠN MỚI HOẶC NỢ: Tất cả các dòng đều tính là chưa thanh toán
                        tongTienChuaThanhToan += thanhTienDong;
                    }
                }
            }

            // Hiển thị số tiền CHƯA THANH TOÁN lên TextBox
            txtTongTien.Text = tongTienChuaThanhToan.ToString("N0");
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

                    // Đọc Trạng thái và Tổng tiền cũ gán vào biến
                    lblTrangThai.Text = readerHD["TrangThai"].ToString().Trim();
                    tongTienCu = Convert.ToDecimal(readerHD["TongTien"]);
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

                    // Gán dữ liệu như cũ
                    row.Cells[0].Value = readerCT["MaSP"].ToString().Trim();
                    row.Cells[1].Value = readerCT["MaSize"].ToString().Trim();
                    row.Cells[2].Value = readerCT["MaMau"].ToString().Trim();
                    row.Cells[3].Value = readerCT["SoLuongBan"].ToString().Trim();
                    row.Cells[4].Value = Convert.ToDecimal(readerCT["GiaBan"]);
                    row.Cells[5].Value = Convert.ToDecimal(readerCT["ThanhTien"]);


                    // Đánh dấu "Cũ" (Đã nằm trong SQL)
                    row.Tag = "Old";
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
        // NÚT THANH TOÁN (Lưu đồng bộ SQL)
        // 2. CẬP NHẬT LẠI NÚT THANH TOÁN (button3_Click)
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHD.Text))
            {
                MessageBox.Show("Vui lòng nhập Mã HDB", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNhanVien.Text))
            {
                MessageBox.Show("Mã nhân viên không được để trống", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNhanVien.Focus();
                return;
            }
            if (cboKhachHang.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn Khách Hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboKhachHang.Focus();
                return;
            }

            // =========================================================
            // 1. KIỂM TRA HÓA ĐƠN TRỐNG (ĐỂ XÓA)
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
                                // HOÀN TRẢ TỒN KHO TRƯỚC KHI XÓA HÓA ĐƠN
                                string sqlHoanTra = @"UPDATE spct SET spct.SoLuongTon = spct.SoLuongTon + ct.SoLuongBan 
                                              FROM tblSanPhamChiTiet spct INNER JOIN tblChiTietHDB ct ON spct.MaSPCT = ct.MaSPCT 
                                              WHERE ct.MaHDB = @MaHDB";
                                SqlCommand cmdHoanTra = new SqlCommand(sqlHoanTra, conn, trans);
                                cmdHoanTra.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                                cmdHoanTra.ExecuteNonQuery();

                                SqlCommand cmdDelCT = new SqlCommand("DELETE FROM tblChiTietHDB WHERE MaHDB = @MaHDB", conn, trans);
                                cmdDelCT.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                                cmdDelCT.ExecuteNonQuery();

                                SqlCommand cmdDelHD = new SqlCommand("DELETE FROM tblHoaDonBan WHERE MaHDB = @MaHDB", conn, trans);
                                cmdDelHD.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                                cmdDelHD.ExecuteNonQuery();

                                trans.Commit();
                                MessageBox.Show("Đã xóa hóa đơn trống khỏi hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                btnHuy_Click(sender, e);
                            }
                            catch { trans.Rollback(); throw; }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Lỗi khi xóa hóa đơn trống: " + ex.Message); }
                    finally { conn.Close(); }
                }
                return;
            }

            // =========================================================
            // 2. LOGIC XỬ LÝ SỐ TIỀN & TRẠNG THÁI (CƠ CHẾ LABEL)
            // =========================================================
            decimal tongTienTrenGiaoDien = 0;
            // Bỏ dấu phẩy của định dạng N0 (nếu có) để parse thành số chuẩn
            decimal.TryParse(txtTongTien.Text.Replace(",", ""), out tongTienTrenGiaoDien);

            // Chặn nếu đơn đã thanh toán mà không mua thêm gì
            if (lblTrangThai.Text == "Đã thanh toán" && tongTienTrenGiaoDien == 0)
            {
                MessageBox.Show("Đơn hàng này đã được thanh toán trước đó!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Tính toán số tiền sẽ lưu xuống DB
            decimal tongTienLuuDB = 0;
            if (lblTrangThai.Text == "Đã thanh toán")
            {
                // Cộng dồn tiền cũ và tiền khách mua thêm
                tongTienLuuDB = tongTienCu + tongTienTrenGiaoDien;
            }
            else
            {
                // Đơn tạo mới hoặc thu nợ -> Lưu đúng số trên màn hình
                tongTienLuuDB = tongTienTrenGiaoDien;
            }

            DialogResult xacNhanThanhToan = MessageBox.Show($"Xác nhận thanh toán đơn hàng này?\n\nSố tiền thu lần này: {tongTienTrenGiaoDien:N0} VNĐ", "Thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhanThanhToan == DialogResult.No) return;

            // =========================================================
            // 3. GHI XUỐNG CƠ SỞ DỮ LIỆU (CÓ CẬP NHẬT KHO)
            // =========================================================
            SqlTransaction transaction = null;
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                transaction = conn.BeginTransaction();

                // Xử lý bảng tblHoaDonBan
                if (KiemTraHoaDonTonTai(txtMaHD.Text.Trim(), transaction))
                {
                    string update = "UPDATE tblHoaDonBan SET MaKH=@MaKH, MaNV=@MaNV, NgayBan=@NgayBan, TrangThai=@TrangThai, TongTien=@TongTien WHERE MaHDB=@MaHDB";
                    SqlCommand cmd = new SqlCommand(update, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", "Đã thanh toán");
                    cmd.Parameters.AddWithValue("@TongTien", tongTienLuuDB);
                    cmd.ExecuteNonQuery();

                    // QUAN TRỌNG: Hoàn trả lại tồn kho của các sản phẩm cũ trước khi ghi đè
                    string sqlHoanTra = @"UPDATE spct SET spct.SoLuongTon = spct.SoLuongTon + ct.SoLuongBan 
                                  FROM tblSanPhamChiTiet spct INNER JOIN tblChiTietHDB ct ON spct.MaSPCT = ct.MaSPCT 
                                  WHERE ct.MaHDB = @MaHDB";
                    SqlCommand cmdHoanTra = new SqlCommand(sqlHoanTra, conn, transaction);
                    cmdHoanTra.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmdHoanTra.ExecuteNonQuery();

                    // Xóa chi tiết cũ để ghi mới
                    SqlCommand cmdDel = new SqlCommand("DELETE FROM tblChiTietHDB WHERE MaHDB=@MaHDB", conn, transaction);
                    cmdDel.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmdDel.ExecuteNonQuery();
                }
                else
                {
                    string insert = "INSERT INTO tblHoaDonBan(MaHDB, MaKH, MaNV, NgayBan, TrangThai, TongTien) VALUES(@MaHDB, @MaKH, @MaNV, @NgayBan, @TrangThai, @TongTien)";
                    SqlCommand cmd = new SqlCommand(insert, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", "Đã thanh toán");
                    cmd.Parameters.AddWithValue("@TongTien", tongTienLuuDB);
                    cmd.ExecuteNonQuery();
                }

                // Ghi chi tiết mới vào tblChiTietHDB và TRỪ TỒN KHO
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    decimal thanhTienRow = 0;
                    if (row.Cells[5].Value != null) decimal.TryParse(row.Cells[5].Value.ToString(), out thanhTienRow);

                    // Chặn lưu những dòng hết hàng (Thành tiền = 0) hoặc chưa dò giá
                    if (!row.IsNewRow && row.Cells[0].Value != null && thanhTienRow > 0)
                    {
                        string qSPCT = "SELECT TOP 1 MaSPCT FROM tblSanPhamChiTiet WHERE MaSP LIKE @MaSP AND MaSize LIKE @Size AND MaMau LIKE @Mau";
                        SqlCommand cmdFind = new SqlCommand(qSPCT, conn, transaction);
                        cmdFind.Parameters.AddWithValue("@MaSP", "%" + row.Cells[0].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Size", "%" + row.Cells[1].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Mau", "%" + row.Cells[2].Value.ToString().Trim() + "%");

                        string maSPCT = cmdFind.ExecuteScalar()?.ToString();
                        if (!string.IsNullOrEmpty(maSPCT))
                        {
                            int slMua = Convert.ToInt32(row.Cells[3].Value);

                            // 1. Thêm vào chi tiết HĐ
                            string insCT = "INSERT INTO tblChiTietHDB(MaHDB, MaSPCT, GiaBan, SoLuongBan) VALUES(@MaHDB, @MaSPCT, @Gia, @SL)";
                            SqlCommand cmdIns = new SqlCommand(insCT, conn, transaction);
                            cmdIns.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                            cmdIns.Parameters.AddWithValue("@MaSPCT", maSPCT);
                            cmdIns.Parameters.AddWithValue("@Gia", Convert.ToDecimal(row.Cells[4].Value));
                            cmdIns.Parameters.AddWithValue("@SL", slMua);
                            cmdIns.ExecuteNonQuery();

                            // 2. Trừ tồn kho thực tế
                            string updateTonKho = "UPDATE tblSanPhamChiTiet SET SoLuongTon = SoLuongTon - @SLMua WHERE MaSPCT = @MaSPCT";
                            SqlCommand cmdTruTon = new SqlCommand(updateTonKho, conn, transaction);
                            cmdTruTon.Parameters.AddWithValue("@SLMua", slMua);
                            cmdTruTon.Parameters.AddWithValue("@MaSPCT", maSPCT);
                            cmdTruTon.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                lblTrangThai.Text = "Đã thanh toán"; // Cập nhật lại giao diện ngay lập tức
                MessageBox.Show("Thanh toán và cập nhật kho thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnHuy_Click(sender, e); // Reset form
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                MessageBox.Show("Lỗi lưu hóa đơn: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            lblTrangThai.Text = "";
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

                    // 3. Load chi tiết sản phẩm 
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
                    MessageBox.Show("Đã tìm thấy mã đơn hàng!", "Thông báo");
                }
                else
                {
                    MessageBox.Show("Mã hóa đơn này chưa tồn tại! Hãy thêm sản phẩm để tạo mới.", "Thông báo");
                    dgvSanPham.Rows.Clear();
                    cboKhachHang.SelectedIndex = -1;
                    txtTongTien.Clear();
                    lblTrangThai.Text = ""; 
                    tongTienCu = 0;         
                }
            }
            catch (Exception ex) { MessageBox.Show("Lỗi tìm kiếm: " + ex.Message); }
            finally { conn.Close(); }
        }
        private void xacnhan_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.Rows.Count == 0 || (dgvSanPham.Rows.Count == 1 && dgvSanPham.Rows[0].IsNewRow))
            {
                MessageBox.Show("Vui lòng chọn sản phẩm trước khi xác nhận!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult xacNhan = MessageBox.Show("Bạn chắc chắn muốn kiểm tra và thêm sản phẩm chứ?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacNhan == DialogResult.Yes)
            {
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    foreach (DataGridViewRow row in dgvSanPham.Rows)
                    {
                        if (!row.IsNewRow && row.Cells[0].Value != null)
                        {
                            // Bỏ qua bước dò giá nếu dòng này là đồ cũ đã lưu (tránh báo lỗi tồn kho sai lệch)
                            if (row.Tag != null && row.Tag.ToString() == "Old") continue;

                            string maSP = row.Cells[0].Value.ToString().Trim();
                            string size = row.Cells[1].Value?.ToString().Trim() ?? "";
                            string mau = row.Cells[2].Value?.ToString().Trim() ?? "";

                            int soLuong = 0;
                            int.TryParse(row.Cells[3].Value?.ToString(), out soLuong);

                            if (string.IsNullOrEmpty(size) || string.IsNullOrEmpty(mau) || soLuong <= 0) continue;

                            // SỬA MỚI: Truy vấn lấy cả Giá Bán và Số Lượng Tồn
                            string queryGia = "SELECT TOP 1 Giaban, SoLuongTon FROM tblSanPhamChiTiet WHERE MaSP LIKE '%' + @MaSP + '%' AND MaSize LIKE '%' + @Size + '%' AND MaMau LIKE '%' + @Mau + '%'";

                            SqlCommand cmdGia = new SqlCommand(queryGia, conn);
                            cmdGia.Parameters.AddWithValue("@MaSP", maSP);
                            cmdGia.Parameters.AddWithValue("@Size", size);
                            cmdGia.Parameters.AddWithValue("@Mau", mau);

                            // SỬA MỚI: Thay ExecuteScalar bằng SqlDataReader để đọc được nhiều cột
                            using (SqlDataReader readerGia = cmdGia.ExecuteReader())
                            {
                                if (readerGia.Read())
                                {
                                    decimal giaBan = Convert.ToDecimal(readerGia["Giaban"]);
                                    int tonKho = Convert.ToInt32(readerGia["SoLuongTon"]);

                                    row.Cells[4].Value = giaBan;

                                    // KIỂM TRA TỒN KHO THỰC TẾ
                                    if (tonKho < soLuong)
                                    {
                                        MessageBox.Show($"Sản phẩm {maSP} (Size {size}, Màu {mau}) chỉ còn {tonKho} cái trong kho!\nKhông đủ số lượng để bán.", "Hết hàng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                        row.Cells[5].Value = 0; // Ép thành tiền về 0
                                    }
                                    else
                                    {
                                        row.Cells[5].Value = giaBan * soLuong; // Tính tiền bình thường
                                    }
                                }
                                else
                                {
                                    // QUAN TRỌNG: Phải đóng luồng đọc trước khi chạy lệnh SQL "Thám tử" đếm dòng
                                    readerGia.Close();

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
                    }

                    TinhTongTien();
                    // Chỉ báo thành công khi hoàn tất vòng lặp
                    MessageBox.Show("Hoàn tất quá trình dò giá và kiểm tra tồn kho!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        private void ResetGiaoDien()
        {
            txtMaHD.Clear();
            txtNhanVien.Clear();
            cboKhachHang.SelectedIndex = -1;
            dtpNgay.Value = DateTime.Now;
            dgvSanPham.Rows.Clear();
            txtTongTien.Clear();
            txtMaHD.Focus();
        }
        private void xoaHoaDon_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem đã có Mã hóa đơn để xóa chưa
            string maHD = txtMaHD.Text.Trim();
            if (string.IsNullOrEmpty(maHD))
            {
                MessageBox.Show("Vui lòng nhập hoặc tìm Mã hóa đơn cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Hiện Pop-up xác nhận
            DialogResult dr = MessageBox.Show($"Bạn chắc chắn muốn xóa hóa đơn {maHD} này chứ?\nThao tác này sẽ xóa toàn bộ sản phẩm liên quan trong kho dữ liệu.",
                                              "Xác nhận xóa",
                                              MessageBoxButtons.YesNo,
                                              MessageBoxIcon.Warning);

            if (dr == DialogResult.Yes)
            {
                SqlTransaction trans = null;
                try
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    trans = conn.BeginTransaction();

                    // Bước A: Xóa toàn bộ sản phẩm trong bảng Chi tiết (tblChiTietHDB)
                    string sqlDeleteDetails = "DELETE FROM tblChiTietHDB WHERE MaHDB = @MaHDB";
                    SqlCommand cmdDetails = new SqlCommand(sqlDeleteDetails, conn, trans);
                    cmdDetails.Parameters.AddWithValue("@MaHDB", maHD);
                    cmdDetails.ExecuteNonQuery();

                    // Bước B: Xóa hóa đơn trong bảng chính (tblHoaDonBan)
                    string sqlDeleteMaster = "DELETE FROM tblHoaDonBan WHERE MaHDB = @MaHDB";
                    SqlCommand cmdMaster = new SqlCommand(sqlDeleteMaster, conn, trans);
                    cmdMaster.Parameters.AddWithValue("@MaHDB", maHD);
                    int rowsAffected = cmdMaster.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        trans.Commit();
                        MessageBox.Show("Đã xóa hóa đơn thành công khỏi hệ thống!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // 3. Reset giao diện sau khi xóa thành công
                        ResetGiaoDien();
                    }
                    else
                    {
                        trans.Rollback();
                        MessageBox.Show("Không tìm thấy mã hóa đơn này trong hệ thống để xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    if (trans != null) trans.Rollback();
                    MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private void btnThanhToanSau_Click(object sender, EventArgs e)
        {
            // 1. Chốt chặn điều kiện áp dụng
            string trangThaiHienTai = lblTrangThai.Text.Trim();
            if (trangThaiHienTai.ToLower() == "đã thanh toán")
            {
                MessageBox.Show("Hóa đơn này đã được thanh toán hoàn tất. Không thể chuyển lùi về trạng thái 'Chưa thanh toán'!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // 2. Kiểm tra thông tin bắt buộc
            if (string.IsNullOrWhiteSpace(txtMaHD.Text) || string.IsNullOrWhiteSpace(txtNhanVien.Text) || cboKhachHang.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã HĐ, Nhân viên và chọn Khách hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. Kiểm tra lưới có sản phẩm không
            bool isEmpty = true;
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (!row.IsNewRow && row.Cells[0].Value != null) { isEmpty = false; break; }
            }
            if (isEmpty)
            {
                MessageBox.Show("Vui lòng thêm sản phẩm trước khi chọn Thanh toán sau!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Xác nhận lưu nợ
            DialogResult dr = MessageBox.Show("Lưu hóa đơn này vào danh sách 'Chưa thanh toán'?", "Xác nhận ghi nợ", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No) return;

            // 5. TIẾN HÀNH LƯU VÀO CSDL
            SqlTransaction transaction = null;
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                transaction = conn.BeginTransaction();

                decimal tongTienLuuDB = 0;
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (!row.IsNewRow && row.Cells[5].Value != null)
                    {
                        tongTienLuuDB += Convert.ToDecimal(row.Cells[5].Value);
                    }
                }

                // Xử lý bảng tblHoaDonBan
                if (KiemTraHoaDonTonTai(txtMaHD.Text.Trim(), transaction))
                {
                    string update = "UPDATE tblHoaDonBan SET MaKH=@MaKH, MaNV=@MaNV, NgayBan=@NgayBan, TrangThai=@TrangThai, TongTien=@TongTien WHERE MaHDB=@MaHDB";
                    SqlCommand cmd = new SqlCommand(update, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", "Chưa thanh toán");
                    cmd.Parameters.AddWithValue("@TongTien", tongTienLuuDB); // Truyền tổng tiền chuẩn xác vào SQL
                    cmd.ExecuteNonQuery();

                    // Hoàn trả kho cũ trước khi ghi đè
                    string sqlHoanTra = @"UPDATE spct SET spct.SoLuongTon = spct.SoLuongTon + ct.SoLuongBan 
                                  FROM tblSanPhamChiTiet spct INNER JOIN tblChiTietHDB ct ON spct.MaSPCT = ct.MaSPCT 
                                  WHERE ct.MaHDB = @MaHDB";
                    SqlCommand cmdHoanTra = new SqlCommand(sqlHoanTra, conn, transaction);
                    cmdHoanTra.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmdHoanTra.ExecuteNonQuery();

                    // Xóa chi tiết cũ
                    SqlCommand cmdDel = new SqlCommand("DELETE FROM tblChiTietHDB WHERE MaHDB=@MaHDB", conn, transaction);
                    cmdDel.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmdDel.ExecuteNonQuery();
                }
                else
                {
                    // Thêm hóa đơn mới
                    string insert = "INSERT INTO tblHoaDonBan(MaHDB, MaKH, MaNV, NgayBan, TrangThai, TongTien) VALUES(@MaHDB, @MaKH, @MaNV, @NgayBan, @TrangThai, @TongTien)";
                    SqlCommand cmd = new SqlCommand(insert, conn, transaction);
                    cmd.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaKH", cboKhachHang.SelectedValue);
                    cmd.Parameters.AddWithValue("@MaNV", txtNhanVien.Text);
                    cmd.Parameters.AddWithValue("@NgayBan", dtpNgay.Value);
                    cmd.Parameters.AddWithValue("@TrangThai", "Chưa thanh toán");
                    cmd.Parameters.AddWithValue("@TongTien", tongTienLuuDB); // Truyền tổng tiền chuẩn xác vào SQL
                    cmd.ExecuteNonQuery();
                }

                // Ghi chi tiết mới và TRỪ TỒN KHO
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    decimal thanhTienRow = 0;
                    if (row.Cells[5].Value != null) decimal.TryParse(row.Cells[5].Value.ToString(), out thanhTienRow);

                    // Chặn lưu những dòng hết hàng (Thành tiền = 0)
                    if (!row.IsNewRow && row.Cells[0].Value != null && thanhTienRow > 0)
                    {
                        string qSPCT = "SELECT TOP 1 MaSPCT FROM tblSanPhamChiTiet WHERE MaSP LIKE @MaSP AND MaSize LIKE @Size AND MaMau LIKE @Mau";
                        SqlCommand cmdFind = new SqlCommand(qSPCT, conn, transaction);
                        cmdFind.Parameters.AddWithValue("@MaSP", "%" + row.Cells[0].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Size", "%" + row.Cells[1].Value.ToString().Trim() + "%");
                        cmdFind.Parameters.AddWithValue("@Mau", "%" + row.Cells[2].Value.ToString().Trim() + "%");

                        string maSPCT = cmdFind.ExecuteScalar()?.ToString();
                        if (!string.IsNullOrEmpty(maSPCT))
                        {
                            int slMua = Convert.ToInt32(row.Cells[3].Value);

                            string insCT = "INSERT INTO tblChiTietHDB(MaHDB, MaSPCT, GiaBan, SoLuongBan) VALUES(@MaHDB, @MaSPCT, @Gia, @SL)";
                            SqlCommand cmdIns = new SqlCommand(insCT, conn, transaction);
                            cmdIns.Parameters.AddWithValue("@MaHDB", txtMaHD.Text.Trim());
                            cmdIns.Parameters.AddWithValue("@MaSPCT", maSPCT);
                            cmdIns.Parameters.AddWithValue("@Gia", Convert.ToDecimal(row.Cells[4].Value));
                            cmdIns.Parameters.AddWithValue("@SL", slMua);
                            cmdIns.ExecuteNonQuery();

                            string updateTonKho = "UPDATE tblSanPhamChiTiet SET SoLuongTon = SoLuongTon - @SLMua WHERE MaSPCT = @MaSPCT";
                            SqlCommand cmdTruTon = new SqlCommand(updateTonKho, conn, transaction);
                            cmdTruTon.Parameters.AddWithValue("@SLMua", slMua);
                            cmdTruTon.Parameters.AddWithValue("@MaSPCT", maSPCT);
                            cmdTruTon.ExecuteNonQuery();
                        }
                    }
                }

                transaction.Commit();
                lblTrangThai.Text = "Chưa thanh toán";
                MessageBox.Show("Hóa đơn đã được chuyển thanh toán sau!\nTổng giá trị hóa đơn: " + tongTienLuuDB.ToString("N0") + " VNĐ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnHuy_Click(sender, e); // Reset form
            }
            catch (Exception ex)
            {
                if (transaction != null) transaction.Rollback();
                MessageBox.Show("Lỗi ghi nợ: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { conn.Close(); }
        }
        private void btnInHoaDon_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra xem người dùng đã chọn mã hóa đơn nào chưa
            string maHD = txtMaHD.Text.Trim();
            if (string.IsNullOrWhiteSpace(maHD))
            {
                MessageBox.Show("Vui lòng nhập hoặc tìm Mã Hóa Đơn cần in trước!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMaHD.Focus();
                return;
            }

            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();

                // 2. Gọi Stored Procedure lấy dữ liệu của RIÊNG hóa đơn này
                SqlCommand cmd = new SqlCommand("sp_LayTatCaHoaDon", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                //cmd.Parameters.AddWithValue("@MaHDB", maHD); // Truyền mã hóa đơn vào SQL

                // Dùng DataAdapter hốt dữ liệu vào DataTable
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dtHoaDonIn = new DataTable();
                da.Fill(dtHoaDonIn);

                // Nếu hóa đơn trống (không có sản phẩm)
                if (dtHoaDonIn.Rows.Count == 0)
                {
                    MessageBox.Show("Hóa đơn này không có sản phẩm nào để in hoặc không tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // =========================================================
                // 3. ĐẨY DỮ LIỆU SANG CRYSTAL REPORT VÀ HIỂN THỊ
                // =========================================================
                // Mở cái Form khung kính mà chúng ta vừa tạo
                frmInHoaDon frmIn = new frmInHoaDon();

                // Khởi tạo file Bản vẽ Hóa đơn (GIẢ SỬ BẠN ĐẶT TÊN FILE LÀ rptHoaDon)
                // Nếu bạn đặt tên khác, hãy sửa lại chữ rptHoaDon ở dòng này cho đúng nhé!
                DanhSachHoaDon rpt = new DanhSachHoaDon();

                // Ép cục dữ liệu DataTable vào Bản vẽ
                rpt.SetDataSource(dtHoaDonIn);

                // Đẩy Bản vẽ lên Khung kính của Form hiển thị
                frmIn.crystalReportViewer1.ReportSource = rpt;
                frmIn.crystalReportViewer1.Refresh();

                // Bật Form lên toàn màn hình cho khách xem
                frmIn.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết xuất hóa đơn: " + ex.Message, "Lỗi in", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }
        }
        private void lblTrangThai_Click(object sender, EventArgs e){}
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