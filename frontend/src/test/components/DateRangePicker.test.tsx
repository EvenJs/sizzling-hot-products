import { describe, it, expect, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import DateRangePicker from "../../components/DateRangePicker";

describe("DateRangePicker", () => {
  it("renders the label correctly", () => {
    render(
      <DateRangePicker label="From" value="2026-04-21" onChange={() => {}} />,
    );
    expect(screen.getByText("From")).toBeInTheDocument();
  });

  it("renders the input with correct value", () => {
    render(
      <DateRangePicker label="From" value="2026-04-21" onChange={() => {}} />,
    );
    const input = screen.getByDisplayValue("2026-04-21");
    expect(input).toBeInTheDocument();
  });

  it("calls onChange with new date when input changes", async () => {
    const handleChange = vi.fn();
    render(
      <DateRangePicker
        label="From"
        value="2026-04-21"
        onChange={handleChange}
      />,
    );

    const input = screen.getByDisplayValue("2026-04-21");
    fireEvent.change(input, { target: { value: "2026-04-23" } });

    expect(handleChange).toHaveBeenCalledWith("2026-04-23");
  });
});
