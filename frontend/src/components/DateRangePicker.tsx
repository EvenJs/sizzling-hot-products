interface Props {
  label: string;
  value: string;
  onChange: (date: string) => void;
}
export default function DateRangePicker({ label, value, onChange }: Props) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1">
        {label}
      </label>
      <input
        type="date"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        className="border rounded p-2"
      />
    </div>
  );
}
