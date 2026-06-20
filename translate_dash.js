const fs = require('fs');
const path = require('path');

const replacements = {
  "Lam moi": "Làm mới",
  "Ap dung": "Áp dụng",
  "Canh bao kho": "Cảnh báo kho",
  "Mon ban chay": "Món bán chạy",
  "don hang": "đơn hàng",
  "Chi so tong hop": "Chỉ số tổng hợp",
  "Doanh thu trung binh/don": "Doanh thu trung bình/đơn",
  "Online": "Trực tuyến"
};

function walk(dir) {
  const list = fs.readdirSync(dir);
  list.forEach((file) => {
    file = path.join(dir, file);
    if (fs.statSync(file).isDirectory()) {
      walk(file);
    } else if (file.endsWith('.ts') || file.endsWith('.html')) {
      let content = fs.readFileSync(file, 'utf8');
      let original = content;

      const keys = Object.keys(replacements).sort((a, b) => b.length - a.length);
      keys.forEach(key => {
        // Regex to avoid matching inside object keys or property names roughly.
        // It's safer to just do global replace since these are mostly UI texts.
        const regex = new RegExp(key.replace(/[.*+?^${}()|[\]\\]/g, '\\$&'), 'g');
        content = content.replace(regex, replacements[key]);
      });

      if (content !== original) {
        fs.writeFileSync(file, content, 'utf8');
        console.log(`Updated text: ${file}`);
      }
    }
  });
}

walk(path.join(__dirname, 'src', 'frontend', 'coffee-chain-admin', 'src', 'app'));
console.log("Translation 3 done.");
