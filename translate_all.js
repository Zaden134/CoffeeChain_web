const fs = require('fs');
const path = require('path');

const replacements = {
  "Tao campaign": "Tạo khuyến mãi",
  "Validation ngay va discount duoc kiem tra o frontend va backend.": "Validation ngày và discount được kiểm tra ở frontend và backend.",
  "Ngay bat dau": "Ngày bắt đầu",
  "Ngay ket thuc": "Ngày kết thúc",
  "Dang kich hoat": "Đang kích hoạt",
  "Tao yeu cau": "Tạo yêu cầu",
  "Chi nhanh": "Chi nhánh",
  "Vi tri": "Vị trí",
  "So luong": "Số lượng",
  "Gui yeu cau": "Gửi yêu cầu",
  "Doanh thu hom nay": "Doanh thu hôm nay",
  "Quan ly": "Quản lý",
  "Hoat dong": "Hoạt động",
  "Chon chi nhanh": "Chọn chi nhánh"
};

function walk(dir) {
  const list = fs.readdirSync(dir);
  list.forEach((file) => {
    file = path.join(dir, file);
    if (fs.statSync(file).isDirectory()) {
      walk(file);
    } else if (file.endsWith('.html') || file.endsWith('.ts')) {
      let content = fs.readFileSync(file, 'utf8');
      let original = content;

      const keys = Object.keys(replacements).sort((a, b) => b.length - a.length);
      keys.forEach(key => {
        const regex = new RegExp(key.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
        content = content.replace(regex, replacements[key]);
      });

      if (content !== original) {
        fs.writeFileSync(file, content, 'utf8');
        console.log(`Updated text in: ${file}`);
      }
    }
  });
}

walk(path.join(__dirname, 'src', 'frontend', 'coffee-chain-admin', 'src', 'app'));
console.log("Translation 4 done.");
