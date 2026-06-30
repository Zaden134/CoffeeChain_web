import { expect, test } from '@playwright/test';

async function login(page: import('@playwright/test').Page, username: string, password: string) {
  await page.goto('/login');
  await page.getByLabel(/Tên đăng nhập/i).fill(username);
  await page.getByLabel(/Mật khẩu/i).fill(password);
  await page.getByRole('button', { name: /Đăng nhập/i }).click();
}

test('branch manager submits recruitment request and admin reviews it', async ({ page }) => {
  const position = `E2E Barista ${Date.now()}`;

  await login(page, 'manager.q1', 'Manager@123');
  await expect(page.getByRole('heading', { name: /Bảng điều khiển/i })).toBeVisible();
  await page.getByRole('link', { name: /Tuyển dụng/i }).click();
  await page.locator('input[name="positionTitle"]').fill(position);
  await page.locator('input[name="quantity"]').fill('1');
  await page.locator('textarea[name="reason"]').fill('Bổ sung nhân sự test E2E.');
  await page.getByRole('button', { name: /Gửi yêu cầu/i }).click();
  await expect(page.getByText(position)).toBeVisible();

  await page.getByRole('button', { name: /Đăng xuất/i }).click();
  await login(page, 'admin', 'Admin@123');
  await page.getByRole('link', { name: /Tuyển dụng/i }).click();
  await page.getByPlaceholder(/Chi nhánh, vị trí, trạng thái/i).fill(position);
  await expect(page.getByText(position)).toBeVisible();
  await page.getByRole('button', { name: /Phê duyệt/i }).first().click();
  await expect(page.getByText(/Approved|Đã duyệt|Approved/i)).toBeVisible();
});
