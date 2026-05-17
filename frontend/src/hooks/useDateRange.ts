import { useEffect, useState } from "react";

export function useDateRange(defaultFrom: string, defaultTo: string) {
  const params = new URLSearchParams(window.location.search);

  const [from, setFrom] = useState(params.get("from") ?? defaultFrom);
  const [to, setTo] = useState(params.get("to") ?? defaultTo);
  const [validationError, setValidationError] = useState<string | null>(null);


  useEffect(() => {
    const params = new URLSearchParams();
    params.set("from", from);
    params.set("to", to);
    window.history.replaceState({}, "", `?${params.toString()}`);
  }, [from, to]);

  const handleFromChange = (date: string) => {
    setFrom(date);
    if (date > to) {
      setValidationError("'From' date must be before 'To' date.");
    } else {
      setValidationError(null);
    }
  };

  const handleToChange = (date: string) => {
    setTo(date);
    if (from > date) {
      setValidationError("'To' date must be after 'From' date.");
    } else {
      setValidationError(null);
    }
  };

  return { from, to, validationError, handleFromChange, handleToChange };
}