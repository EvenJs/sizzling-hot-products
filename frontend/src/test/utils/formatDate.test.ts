import { describe, it, expect } from 'vitest';
import { formatDate } from '../../utils/dateFormatter';

describe('formatDate', () => {
  it('formats ISO date string to Australian format', () => {
    expect(formatDate('2026-04-21')).toBe('21/04/2026');
  });

  it('formats different valid date correctly', () => {
    expect(formatDate('2026-04-23')).toBe('23/04/2026');
  });
});