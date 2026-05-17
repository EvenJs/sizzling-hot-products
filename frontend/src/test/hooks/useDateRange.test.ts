import { renderHook, act } from '@testing-library/react';
import { useDateRange } from '../../hooks/useDateRange';
import { describe, expect, it } from 'vitest';

describe('useDateRange', () => {
  it('returns default dates on init', () => {
    const { result } = renderHook(() =>
      useDateRange('2026-04-21', '2026-04-23')
    );

    expect(result.current.from).toBe('2026-04-21');
    expect(result.current.to).toBe('2026-04-23');
  });

  it('sets validation error when from is after to', () => {
    const { result } = renderHook(() =>
      useDateRange('2026-04-21', '2026-04-23')
    );

    act(() => {
      result.current.handleFromChange('2026-04-25');
    });

    expect(result.current.validationError).toBe("'From' date must be before 'To' date.");
  });

  it('clears validation error when dates become valid', () => {
    const { result } = renderHook(() =>
      useDateRange('2026-04-21', '2026-04-23')
    );

    act(() => {
      result.current.handleFromChange('2026-04-25');
    });

    act(() => {
      result.current.handleFromChange('2026-04-21');
    });

    expect(result.current.validationError).toBeNull();
  });
});