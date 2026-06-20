const fs = require('fs');
const path = require('path');

const replacements = {
  "Tao muc ton kho": "Tạo mục tồn kho",
  "Ton kho duoc doc va ghi truc tiep tu backend PostgreSQL.": "Tồn kho được đọc và ghi trực tiếp từ backend PostgreSQL.",
  "Tim kiem": "Tìm kiếm",
  "Truoc": "Trước",
  "Sau": "Sau",
  "Dang tai ton kho...": "Đang tải tồn kho...",
  "Ton kho": "Tồn kho",
  "Dat truoc": "Đặt trước",
  "Muc canh bao": "Mức cảnh báo",
  "Low stock": "Sắp hết",
  "On track": "Còn hàng",
  "Sua": "Sửa",
  "Xoa": "Xóa",
  "Cap nhat ton kho": "Cập nhật tồn kho",
  "Tao ton kho": "Tạo tồn kho",
  "Co the chon ingredient san co hoac tao ingredient moi ngay trong form.": "Có thể chọn nguyên liệu sẵn có hoặc tạo nguyên liệu mới ngay trong form.",
  "Ingredient san co": "Nguyên liệu sẵn có",
  "Tao ingredient moi": "Tạo nguyên liệu mới",
  "Ten ingredient": "Tên nguyên liệu",
  "Don vi": "Đơn vị",
  "Ingredient, chi nhanh, don vi...": "Nguyên liệu, chi nhánh, đơn vị...",

  "Ten, email, username...": "Tên, email, username...",
  "Dang tai nhan vien...": "Đang tải nhân viên...",
  "Vo hieu hoa": "Vô hiệu hóa",
  "Cap nhat nhan vien": "Cập nhật nhân viên",
  "Tao nhan vien": "Tạo nhân viên",
  "Mat khau co the bo trong khi sua neu khong doi.": "Mật khẩu có thể bỏ trống khi sửa nếu không đổi.",
  "Ho ten": "Họ tên",
  "Vai tro": "Vai trò",
  "Dang hoat dong": "Đang hoạt động",
  "de trong neu khong doi": "để trống nếu không đổi",
  "Mat khau": "Mật khẩu",

  "Tao chi nhanh": "Tạo chi nhánh",
  "Ten chi nhanh": "Tên chi nhánh",
  "Dia chi": "Địa chỉ",
  "Cap nhat chi nhanh": "Cập nhật chi nhánh",
  "Khong tai duoc chi nhanh.": "Không tải được chi nhánh.",
  "Dang tai chi nhanh...": "Đang tải chi nhánh...",

  "Tao san pham": "Tạo sản phẩm",
  "Cap nhat san pham": "Cập nhật sản phẩm",
  "Dang tai san pham...": "Đang tải sản phẩm...",
  "Mo ta": "Mô tả",
  "Gia": "Giá",

  "Tao khuyen mai": "Tạo khuyến mãi",
  "Cap nhat khuyen mai": "Cập nhật khuyến mãi",
  "Dang tai khuyen mai...": "Đang tải khuyến mãi...",

  "Phe duyet": "Phê duyệt",
  "Tu choi": "Từ chối",
  "Cho xac nhan": "Chờ xác nhận",
  "Da duyet": "Đã duyệt",

  "Pending": "Chờ duyệt",
  "Approved": "Đã duyệt",
  "Rejected": "Từ chối",
  "Active": "Hoạt động",
  "Inactive": "Đã khóa",

  "Trang thai": "Trạng thái",
  "Ly do": "Lý do",
  "Huy": "Hủy",
  "Luu": "Lưu",
  "Dang luu...": "Đang lưu...",
  "Cap nhat": "Cập nhật",
  "Tao moi": "Tạo mới"
};

function walk(dir) {
  let results = [];
  const list = fs.readdirSync(dir);
  list.forEach((file) => {
    file = path.join(dir, file);
    const stat = fs.statSync(file);
    if (stat && stat.isDirectory()) {
      results = results.concat(walk(file));
    } else {
      if (file.endsWith('.ts') || file.endsWith('.html')) {
        results.push(file);
      }
    }
  });
  return results;
}

const targetDir = path.join(__dirname, 'src', 'frontend', 'coffee-chain-admin', 'src', 'app');
const files = walk(targetDir);

files.forEach(file => {
  let content = fs.readFileSync(file, 'utf8');
  let original = content;

  // Sort by length to match longer strings first
  const keys = Object.keys(replacements).sort((a, b) => b.length - a.length);

  keys.forEach(key => {
    const regex = new RegExp(key.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
    content = content.replace(regex, replacements[key]);
  });

  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log(`Updated: ${file}`);
  }
});
console.log("Translation done.");
