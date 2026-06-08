const fs = require('fs');
const path = require('path');

const replacements = {
  // Navigation & Layout
  "Chi nhanh": "Chi nhánh",
  "San pham": "Sản phẩm",
  "Kho hang": "Kho hàng",
  "Nhan vien": "Nhân viên",
  "Bao cao": "Báo cáo",
  "Khuyen mai": "Khuyến mãi",
  "Yeu cau tuyen dung": "Yêu cầu tuyển dụng",
  "Yeu cau tuyen": "Yêu cầu tuyển",
  "Chuoi cua hang ca phe": "Chuỗi cửa hàng cà phê",
  "He thong quan ly chuoi cua hang ca phe": "Hệ thống quản lý chuỗi cửa hàng cà phê",
  "Van hanh, kho, nhan su va doanh thu trong mot giao dien.": "Vận hành, kho, nhân sự và doanh thu trong một giao diện.",
  "Dang xuat": "Đăng xuất",

  // Products
  "Dang tai san pham...": "Đang tải sản phẩm...",
  "Khong tai duoc danh sach san pham.": "Không tải được danh sách sản phẩm.",

  // Inventory
  "Chi nhanh, don vi...": "Chi nhánh, đơn vị...",
  "Chon chi nhanh": "Chọn chi nhánh",
  "Chi nhanh la bat buoc.": "Chi nhánh là bắt buộc.",

  // Promotions
  "Dang tai khuyen mai...": "Đang tải khuyến mãi...",
  "Ten khuyen mai": "Tên khuyến mãi",
  "Khong tai duoc danh sach khuyen mai.": "Không tải được danh sách khuyến mãi.",
  "Khong luu duoc khuyen mai.": "Không lưu được khuyến mãi.",
  "Xoa khuyen mai": "Xóa khuyến mãi",
  "Khong xoa duoc khuyen mai.": "Không xóa được khuyến mãi.",
  "Ten khuyen mai la bat buoc.": "Tên khuyến mãi là bắt buộc.",

  // Recruitment Requests
  "Quan ly chi nhanh gui de xuat tuyen nguoi, admin xem va phe duyet.": "Quản lý chi nhánh gửi đề xuất tuyển người, admin xem và phê duyệt.",
  "Chi nhanh, vi tri, trang thai...": "Chi nhánh, vị trí, trạng thái...",
  "Chi quan ly chi nhanh moi duoc tao yeu cau cho chi nhanh cua minh.": "Chỉ quản lý chi nhánh mới được tạo yêu cầu cho chi nhánh của mình.",
  "Khong tai duoc yeu cau tuyen dung.": "Không tải được yêu cầu tuyển dụng.",
  "Khong tao duoc yeu cau tuyen dung.": "Không tạo được yêu cầu tuyển dụng.",
  "Chi nhanh, vi tri va ly do la bat buoc.": "Chi nhánh, vị trí và lý do là bắt buộc.",

  // Reports
  "Dang tai bao cao...": "Đang tải báo cáo...",
  "Bao cao doanh thu": "Báo cáo doanh thu",
  "Loc theo thoi gian va chi nhanh, sau do xuat ra Excel hoac PDF.": "Lọc theo thời gian và chi nhánh, sau đó xuất ra Excel hoặc PDF.",
  "Doanh thu theo chi nhanh": "Doanh thu theo chi nhánh",
  "Chi nhanh hoat dong": "Chi nhánh hoạt động",
  "Yeu cau tuyen dang cho": "Yêu cầu tuyển đang chờ",
  "Chi nhanh da loc": "Chi nhánh đã lọc",
  "Tat ca": "Tất cả",
  "Khong tai duoc bao cao.": "Không tải được báo cáo.",
  "Khong tai duoc file bao cao.": "Không tải được file báo cáo.",
  "Khong xuat duoc file bao cao.": "Không xuất được file báo cáo.",

  // Error/Common
  "Khong xoa duoc nhan vien.": "Không xóa được nhân viên.",
  "Nhan vien khong phai admin phai gan voi mot chi nhanh.": "Nhân viên không phải admin phải gắn với một chi nhánh.",
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

  // Sắp xếp keys theo độ dài giảm dần để match các chuỗi dài trước
  const keys = Object.keys(replacements).sort((a, b) => b.length - a.length);

  keys.forEach(key => {
    // Thay thế tất cả sự xuất hiện của chuỗi
    // Dùng regex với cờ global để thay thế toàn bộ
    // Escaping regex characters in key just in case
    const escapedKey = key.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    const regex = new RegExp(escapedKey, 'g');
    content = content.replace(regex, replacements[key]);
  });

  if (content !== original) {
    fs.writeFileSync(file, content, 'utf8');
    console.log(`Updated: ${file}`);
  }
});

console.log("Done updating Vietnamese text.");
