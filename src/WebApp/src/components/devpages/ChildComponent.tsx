import { useEffect, useState } from "react";

export function ChildComponent() {
  console.log("ChildComponent");
  const [state, setState] = useState(0);
  useEffect(() => {
    console.log("ChildComponent:useEffect");
    setTimeout(() => {
      setState(1);
    }, 100);
  }, []);
  return <div>ChildComponent. state: {state}</div>;
}
