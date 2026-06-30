import { expect, test } from '@playwright/test';

async function loginAsAdmin(page: import('@playwright/test').Page) {
  await page.goto('/login');
  await page.getByLabel(/Tên đăng nhập/i).fill('admin');
  await page.getByLabel(/Mật khẩu/i).fill('Admin@123');
  await page.getByRole('button', { name: /Đăng nhập/i }).click();
  await expect(page.getByRole('heading', { name: /Bảng điều khiển/i })).toBeVisible();
}

test('admin can login and open core management pages', async ({ page }) => {
  await loginAsAdmin(page);

  await page.getByRole('link', { name: /Sản phẩm/i }).click();
  await expect(page.getByRole('heading', { name: /Sản phẩm/i })).toBeVisible();

  await page.getByRole('link', { name: /Báo cáo/i }).click();
  await expect(page.getByRole('heading', { name: /Báo cáo/i })).toBeVisible();

  await page.getByRole('link', { name: /Audit log/i }).click();
  await expect(page.getByRole('heading', { name: /Audit log/i })).toBeVisible();

  await page.getByRole('link', { name: /Bảo mật/i }).click();
  await expect(page.getByRole('heading', { name: /Bảo mật/i })).toBeVisible();
});

test('admin can create and hide a product', async ({ page }) => {
  await loginAsAdmin(page);
  await page.getByRole('link', { name: /Sản phẩm/i }).click();

  const suffix = Date.now();
  const sku = `E2E-${suffix}`;
  await page.getByRole('button', { name: /Thêm sản phẩm/i }).click();
  await page.locator('input[name="sku"]').fill(sku);
  await page.locator('input[name="name"]').fill(`E2E Latte ${suffix}`);
  await page.locator('input[name="category"]').fill('E2E');
  await page.locator('input[name="price"]').fill('59000');
  await page.getByRole('button', { name: /^Lưu$/i }).click();

  await page.getByPlaceholder(/Tìm SKU/i).fill(sku);
  await expect(page.getByText(sku)).toBeVisible();

  await page.getByRole('button', { name: /Sửa/i }).first().click();
  page.once('dialog', (dialog) => dialog.accept());
  await page.getByRole('button', { name: /^Ẩn$/i }).click();
  await expect(page.getByText(/Tạm ẩn/i)).toBeVisible();
});

test('admin can filter reports and request export', async ({ page }) => {
  await loginAsAdmin(page);
  await page.getByRole('link', { name: /Báo cáo/i }).click();
  await expect(page.getByText(/Tổng doanh thu/i)).toBeVisible();

  await page.locator('input[name="fromDate"]').fill('2026-01-01');
  await page.locator('input[name="toDate"]').fill('2026-12-31');
  await page.getByRole('button', { name: /Lọc dữ liệu/i }).click();
  await expect(page.getByText(/Tổng đơn hàng/i)).toBeVisible();

  const downloadPromise = page.waitForEvent('download');
  await page.getByRole('button', { name: /Xuất Excel/i }).click();
  const download = await downloadPromise;
  expect(download.suggestedFilename()).toContain('sales-report');
});
