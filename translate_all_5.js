const fs = require('fs');
const path = require('path');

const replacements = {
  "ly da ban": "ly đã bán",
  "Dang ban": "Đang bán"
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
console.log("Translation 5 done.");
