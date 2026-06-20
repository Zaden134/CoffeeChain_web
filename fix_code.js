const fs = require('fs');
const path = require('path');

function fix(dir) {
  const list = fs.readdirSync(dir);
  list.forEach((file) => {
    file = path.join(dir, file);
    if (fs.statSync(file).isDirectory()) {
      fix(file);
    } else if (file.endsWith('.ts') || file.endsWith('.html')) {
      let content = fs.readFileSync(file, 'utf8');
      let original = content;

      content = content.replace(/isHoạt động/g, 'isActive');
      content = content.replace(/routerLinkHoạt động/g, 'routerLinkActive');
      content = content.replace(/RouterLinkHoạt động/g, 'RouterLinkActive');

      // Revert literal types and logic comparisons
      content = content.replace(/'Chờ duyệt' \| 'Đã duyệt' \| 'Từ chối'/g, "'Pending' | 'Approved' | 'Rejected'");
      content = content.replace(/=== 'Chờ duyệt'/g, "=== 'Pending'");
      content = content.replace(/=== 'Đã duyệt'/g, "=== 'Approved'");
      content = content.replace(/=== 'Từ chối'/g, "=== 'Rejected'");
      content = content.replace(/=== 'Hoạt động'/g, "=== 'Active'");
      content = content.replace(/=== 'Đã khóa'/g, "=== 'Inactive'");
      
      content = content.replace(/!== 'Chờ duyệt'/g, "!== 'Pending'");
      content = content.replace(/!== 'Đã duyệt'/g, "!== 'Approved'");
      content = content.replace(/!== 'Từ chối'/g, "!== 'Rejected'");
      content = content.replace(/!== 'Hoạt động'/g, "!== 'Active'");
      content = content.replace(/!== 'Đã khóa'/g, "!== 'Inactive'");

      // And for the ternary operators that might be affected, but actually I only replaced "Pending" with "Chờ duyệt" everywhere. So ternary `status === 'Pending' ? 'Pending' : ...` became `status === 'Chờ duyệt' ? 'Chờ duyệt' : ...`.
      // I should manually inspect the files or just run npm run build to see.

      if (content !== original) {
        fs.writeFileSync(file, content, 'utf8');
        console.log(`Fixed: ${file}`);
      }
    }
  });
}
fix(path.join(__dirname, 'src', 'frontend', 'coffee-chain-admin', 'src', 'app'));
