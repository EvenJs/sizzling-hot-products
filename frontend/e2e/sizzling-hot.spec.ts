import { test, expect } from '@playwright/test';

test.describe('Sizzling Hot Products', () => {
  test('shows correct default results on load', async ({ page }) => {
    await page.goto('http://localhost:3000');
    await page.waitForLoadState('networkidle');

    // assert expected products are visible
    await expect(page.getByText('Ezy Storage 37L Flexi Laundry Basket - White').first()).toBeVisible();
    await expect(page.getByText('Arlec 160W Crystalline Solar Foldable Charging Kit').first()).toBeVisible();
  });

  test('shows validation error when from is after to', async ({ page }) => {
    await page.goto('http://localhost:3000');

    // set from date after to date
    await page.locator('input[type="date"]').first().fill('2026-04-25');

    await expect(page.getByText("'From' date must be before 'To' date.")).toBeVisible();
  });

  test('URL updates when dates change', async ({ page }) => {
    await page.goto('http://localhost:3000');

    await page.locator('input[type="date"]').first().fill('2026-04-21');

    await expect(page).toHaveURL(/from=2026-04-21/);
  });

  test('changing dates updates the results', async ({ page }) => {
    await page.goto('http://localhost:3000');

    await page.locator('input[type="date"]').first().fill('2026-04-23');
    await page.locator('input[type="date"]').last().fill('2026-04-23');

    await expect(page.getByText('Arlec 160W Crystalline Solar Foldable Charging Kit').first()).toBeVisible();
    await expect(page.getByText('Ezy Storage 37L Flexi Laundry Basket - White').first()).not.toBeVisible();
  });
});

