namespace HSK
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnThanhToanSau = new System.Windows.Forms.Button();
            this.btnInHoaDon = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblTrangThai = new System.Windows.Forms.Label();
            this.xoaHoaDon = new System.Windows.Forms.Button();
            this.xoasp = new System.Windows.Forms.Button();
            this.xacnhan = new System.Windows.Forms.Button();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.btnThanhToan = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.txtTongTien = new System.Windows.Forms.TextBox();
            this.lblTongTien = new System.Windows.Forms.Label();
            this.dgvSanPham = new System.Windows.Forms.DataGridView();
            this.colSanPham = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colMau = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colSoLuong = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGia = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colThanhTien = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtNhanVien = new System.Windows.Forms.TextBox();
            this.lblNV = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cboKhachHang = new System.Windows.Forms.ComboBox();
            this.lblKH = new System.Windows.Forms.Label();
            this.dtpNgay = new System.Windows.Forms.DateTimePicker();
            this.lblNgay = new System.Windows.Forms.Label();
            this.txtMaHD = new System.Windows.Forms.TextBox();
            this.lblMaHD = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPham)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnThanhToanSau);
            this.groupBox1.Controls.Add(this.btnInHoaDon);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblTrangThai);
            this.groupBox1.Controls.Add(this.xoaHoaDon);
            this.groupBox1.Controls.Add(this.xoasp);
            this.groupBox1.Controls.Add(this.xacnhan);
            this.groupBox1.Controls.Add(this.btnTimKiem);
            this.groupBox1.Controls.Add(this.btnThanhToan);
            this.groupBox1.Controls.Add(this.btnHuy);
            this.groupBox1.Controls.Add(this.txtTongTien);
            this.groupBox1.Controls.Add(this.lblTongTien);
            this.groupBox1.Controls.Add(this.dgvSanPham);
            this.groupBox1.Controls.Add(this.txtNhanVien);
            this.groupBox1.Controls.Add(this.lblNV);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cboKhachHang);
            this.groupBox1.Controls.Add(this.lblKH);
            this.groupBox1.Controls.Add(this.dtpNgay);
            this.groupBox1.Controls.Add(this.lblNgay);
            this.groupBox1.Controls.Add(this.txtMaHD);
            this.groupBox1.Controls.Add(this.lblMaHD);
            this.groupBox1.Location = new System.Drawing.Point(7, 1);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(652, 460);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "BÁN HÀNG";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // btnThanhToanSau
            // 
            this.btnThanhToanSau.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnThanhToanSau.Location = new System.Drawing.Point(102, 413);
            this.btnThanhToanSau.Name = "btnThanhToanSau";
            this.btnThanhToanSau.Size = new System.Drawing.Size(103, 23);
            this.btnThanhToanSau.TabIndex = 27;
            this.btnThanhToanSau.Text = "Thanh toán sau ";
            this.btnThanhToanSau.UseVisualStyleBackColor = false;
            this.btnThanhToanSau.Click += new System.EventHandler(this.btnThanhToanSau_Click);
            // 
            // btnInHoaDon
            // 
            this.btnInHoaDon.BackColor = System.Drawing.SystemColors.Info;
            this.btnInHoaDon.Location = new System.Drawing.Point(539, 413);
            this.btnInHoaDon.Name = "btnInHoaDon";
            this.btnInHoaDon.Size = new System.Drawing.Size(75, 23);
            this.btnInHoaDon.TabIndex = 26;
            this.btnInHoaDon.Text = "In hóa đơn";
            this.btnInHoaDon.UseVisualStyleBackColor = false;
            this.btnInHoaDon.Click += new System.EventHandler(this.btnInHoaDon_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(353, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 13);
            this.label3.TabIndex = 25;
            this.label3.Text = "Trạng thái đơn hàng:";
            // 
            // lblTrangThai
            // 
            this.lblTrangThai.BackColor = System.Drawing.Color.White;
            this.lblTrangThai.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTrangThai.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblTrangThai.Location = new System.Drawing.Point(466, 55);
            this.lblTrangThai.Name = "lblTrangThai";
            this.lblTrangThai.Size = new System.Drawing.Size(130, 23);
            this.lblTrangThai.TabIndex = 23;
            this.lblTrangThai.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTrangThai.Click += new System.EventHandler(this.lblTrangThai_Click);
            // 
            // xoaHoaDon
            // 
            this.xoaHoaDon.Location = new System.Drawing.Point(443, 331);
            this.xoaHoaDon.Name = "xoaHoaDon";
            this.xoaHoaDon.Size = new System.Drawing.Size(171, 23);
            this.xoaHoaDon.TabIndex = 22;
            this.xoaHoaDon.Text = "Xóa hóa đơn";
            this.xoaHoaDon.UseVisualStyleBackColor = true;
            this.xoaHoaDon.Click += new System.EventHandler(this.xoaHoaDon_Click);
            // 
            // xoasp
            // 
            this.xoasp.Location = new System.Drawing.Point(494, 302);
            this.xoasp.Name = "xoasp";
            this.xoasp.Size = new System.Drawing.Size(120, 23);
            this.xoasp.TabIndex = 21;
            this.xoasp.Text = "- Xóa sản phẩm";
            this.xoasp.UseVisualStyleBackColor = true;
            this.xoasp.Click += new System.EventHandler(this.xoasp_Click);
            // 
            // xacnhan
            // 
            this.xacnhan.Location = new System.Drawing.Point(11, 302);
            this.xacnhan.Name = "xacnhan";
            this.xacnhan.Size = new System.Drawing.Size(166, 23);
            this.xacnhan.TabIndex = 20;
            this.xacnhan.Text = "+ Xác nhận thêm sản phẩm";
            this.xacnhan.UseVisualStyleBackColor = true;
            this.xacnhan.Click += new System.EventHandler(this.xacnhan_Click);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Location = new System.Drawing.Point(210, 22);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(90, 90);
            this.btnTimKiem.TabIndex = 18;
            this.btnTimKiem.Text = "Tìm Kiếm Mã Hóa Đơn Bán";
            this.btnTimKiem.UseVisualStyleBackColor = true;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_click_Click);
            // 
            // btnThanhToan
            // 
            this.btnThanhToan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnThanhToan.Location = new System.Drawing.Point(11, 413);
            this.btnThanhToan.Name = "btnThanhToan";
            this.btnThanhToan.Size = new System.Drawing.Size(75, 23);
            this.btnThanhToan.TabIndex = 17;
            this.btnThanhToan.Text = "Thanh toán";
            this.btnThanhToan.UseVisualStyleBackColor = false;
            this.btnThanhToan.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.BackColor = System.Drawing.Color.Red;
            this.btnHuy.Location = new System.Drawing.Point(225, 413);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(75, 23);
            this.btnHuy.TabIndex = 16;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = false;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // txtTongTien
            // 
            this.txtTongTien.Location = new System.Drawing.Point(76, 381);
            this.txtTongTien.Name = "txtTongTien";
            this.txtTongTien.ReadOnly = true;
            this.txtTongTien.Size = new System.Drawing.Size(129, 20);
            this.txtTongTien.TabIndex = 15;
            this.txtTongTien.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTongTien.TextChanged += new System.EventHandler(this.txtTongTien_TextChanged);
            // 
            // lblTongTien
            // 
            this.lblTongTien.AutoSize = true;
            this.lblTongTien.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblTongTien.Location = new System.Drawing.Point(8, 384);
            this.lblTongTien.Name = "lblTongTien";
            this.lblTongTien.Size = new System.Drawing.Size(62, 13);
            this.lblTongTien.TabIndex = 14;
            this.lblTongTien.Text = "Tổng Tiền: ";
            this.lblTongTien.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.lblTongTien.Click += new System.EventHandler(this.lblTongTien_Click);
            // 
            // dgvSanPham
            // 
            this.dgvSanPham.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSanPham.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSanPham,
            this.colSize,
            this.colMau,
            this.colSoLuong,
            this.colGia,
            this.colThanhTien});
            this.dgvSanPham.Location = new System.Drawing.Point(11, 128);
            this.dgvSanPham.Name = "dgvSanPham";
            this.dgvSanPham.Size = new System.Drawing.Size(603, 168);
            this.dgvSanPham.TabIndex = 12;
            this.dgvSanPham.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPham_CellContentClick);
            // 
            // colSanPham
            // 
            this.colSanPham.HeaderText = "Sản phẩm";
            this.colSanPham.Items.AddRange(new object[] {
            "Áo thun ",
            "Quần jeans ",
            "Áo sơ mi ",
            "Váy "});
            this.colSanPham.Name = "colSanPham";
            // 
            // colSize
            // 
            this.colSize.HeaderText = "Size";
            this.colSize.Items.AddRange(new object[] {
            "S ",
            "M ",
            "L ",
            "XL "});
            this.colSize.Name = "colSize";
            // 
            // colMau
            // 
            this.colMau.HeaderText = "Màu";
            this.colMau.Items.AddRange(new object[] {
            "Đen ",
            "Trắng ",
            "Đỏ ",
            "Xanh "});
            this.colMau.Name = "colMau";
            // 
            // colSoLuong
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colSoLuong.DefaultCellStyle = dataGridViewCellStyle4;
            this.colSoLuong.HeaderText = "SL";
            this.colSoLuong.Name = "colSoLuong";
            this.colSoLuong.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colSoLuong.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colSoLuong.Width = 60;
            // 
            // colGia
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colGia.DefaultCellStyle = dataGridViewCellStyle5;
            this.colGia.HeaderText = "Giá";
            this.colGia.Name = "colGia";
            // 
            // colThanhTien
            // 
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.colThanhTien.DefaultCellStyle = dataGridViewCellStyle6;
            this.colThanhTien.HeaderText = "Thành tiền";
            this.colThanhTien.Name = "colThanhTien";
            this.colThanhTien.ReadOnly = true;
            // 
            // txtNhanVien
            // 
            this.txtNhanVien.Location = new System.Drawing.Point(77, 92);
            this.txtNhanVien.Name = "txtNhanVien";
            this.txtNhanVien.Size = new System.Drawing.Size(100, 20);
            this.txtNhanVien.TabIndex = 11;
            this.txtNhanVien.TextChanged += new System.EventHandler(this.txtNhanVien_TextChanged);
            // 
            // lblNV
            // 
            this.lblNV.AutoSize = true;
            this.lblNV.Location = new System.Drawing.Point(8, 95);
            this.lblNV.Name = "lblNV";
            this.lblNV.Size = new System.Drawing.Size(43, 13);
            this.lblNV.TabIndex = 10;
            this.lblNV.Text = "Mã NV:";
            this.lblNV.Click += new System.EventHandler(this.lblNV_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(85, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 9;
            // 
            // cboKhachHang
            // 
            this.cboKhachHang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboKhachHang.FormattingEnabled = true;
            this.cboKhachHang.Location = new System.Drawing.Point(76, 57);
            this.cboKhachHang.Name = "cboKhachHang";
            this.cboKhachHang.Size = new System.Drawing.Size(100, 21);
            this.cboKhachHang.TabIndex = 8;
            this.cboKhachHang.SelectedIndexChanged += new System.EventHandler(this.cboKhachHang_SelectedIndexChanged);
            // 
            // lblKH
            // 
            this.lblKH.AutoSize = true;
            this.lblKH.Location = new System.Drawing.Point(8, 60);
            this.lblKH.Name = "lblKH";
            this.lblKH.Size = new System.Drawing.Size(43, 13);
            this.lblKH.TabIndex = 7;
            this.lblKH.Text = "Mã KH:";
            this.lblKH.Click += new System.EventHandler(this.lblKH_Click);
            // 
            // dtpNgay
            // 
            this.dtpNgay.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpNgay.Location = new System.Drawing.Point(416, 24);
            this.dtpNgay.Name = "dtpNgay";
            this.dtpNgay.Size = new System.Drawing.Size(108, 20);
            this.dtpNgay.TabIndex = 6;
            this.dtpNgay.ValueChanged += new System.EventHandler(this.dtpNgay_ValueChanged);
            // 
            // lblNgay
            // 
            this.lblNgay.AutoSize = true;
            this.lblNgay.Location = new System.Drawing.Point(353, 27);
            this.lblNgay.Name = "lblNgay";
            this.lblNgay.Size = new System.Drawing.Size(57, 13);
            this.lblNgay.TabIndex = 4;
            this.lblNgay.Text = "Ngày Bán:";
            this.lblNgay.Click += new System.EventHandler(this.label1_Click);
            // 
            // txtMaHD
            // 
            this.txtMaHD.Location = new System.Drawing.Point(77, 24);
            this.txtMaHD.Name = "txtMaHD";
            this.txtMaHD.Size = new System.Drawing.Size(100, 20);
            this.txtMaHD.TabIndex = 3;
            this.txtMaHD.TextChanged += new System.EventHandler(this.txtMaHD_TextChanged);
            // 
            // lblMaHD
            // 
            this.lblMaHD.AutoSize = true;
            this.lblMaHD.Location = new System.Drawing.Point(8, 27);
            this.lblMaHD.Name = "lblMaHD";
            this.lblMaHD.Size = new System.Drawing.Size(51, 13);
            this.lblMaHD.TabIndex = 2;
            this.lblMaHD.Text = "Mã HDB:";
            this.lblMaHD.Click += new System.EventHandler(this.lblMaHD_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 459);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPham)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblMaHD;
        private System.Windows.Forms.TextBox txtMaHD;
        private System.Windows.Forms.DateTimePicker dtpNgay;
        private System.Windows.Forms.Label lblNgay;
        private System.Windows.Forms.DataGridView dgvSanPham;
        private System.Windows.Forms.TextBox txtNhanVien;
        private System.Windows.Forms.Label lblNV;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboKhachHang;
        private System.Windows.Forms.Label lblKH;
        private System.Windows.Forms.Label lblTongTien;
        private System.Windows.Forms.TextBox txtTongTien;
        private System.Windows.Forms.Button btnThanhToan;
        private System.Windows.Forms.Button btnHuy;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSanPham;
        private System.Windows.Forms.DataGridViewComboBoxColumn colSize;
        private System.Windows.Forms.DataGridViewComboBoxColumn colMau;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSoLuong;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGia;
        private System.Windows.Forms.DataGridViewTextBoxColumn colThanhTien;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.Button xacnhan;
        private System.Windows.Forms.Button xoasp;
        private System.Windows.Forms.Button xoaHoaDon;
        private System.Windows.Forms.Label lblTrangThai;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnInHoaDon;
        private System.Windows.Forms.Button btnThanhToanSau;
    }
}

