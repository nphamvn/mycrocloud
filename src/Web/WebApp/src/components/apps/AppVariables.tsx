import { useEffect, useState } from "react";
import IVariable from "./Variable";

export default function AppVariables() {
  const [variables, setVariables] = useState<IVariable[]>([]);
  useEffect(() => {
    setTimeout(() => {
      setVariables([
        {
          id: 1,
          name: "test",
          value: "test",
          createdAt: new Date().toISOString(),
        },
      ]);
    }, 100);
  }, []);
  return (
    <div className="p-2">
      <h1 className="font-bold">Variables</h1>
      <div className="mt-2">
        <button type="button" className="bg-primary px-3 py-1 text-white">
          Add
        </button>
        <table className="mt-2">
          <thead>
            <tr>
              <th>Name</th>
              <th>Value</th>
              <th>Created at</th>
              <th>Updated at</th>
            </tr>
          </thead>
          <tbody>
            {variables.map((v) => (
              <tr key={v.id}>
                <td>{v.name}</td>
                <td>{v.value}</td>
                <td>{v.createdAt}</td>
                <td>{v.updatedAt}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
