import { useContext, useEffect, useState } from "react";
import IVariable from "./Variable";
import { Link } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";
import { AppContext } from "../apps/AppContext";

export default function AppVariables() {
  const app = useContext(AppContext)!;
  const [variables, setVariables] = useState<IVariable[]>([]);
  const { getAccessTokenSilently } = useAuth0();
  useEffect(() => {
    const getVariable = async () => {
      const accessToken = await getAccessTokenSilently();
      const res = await fetch(`/api/apps/${app.id}/variables`, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
      const variables = (await res.json()) as IVariable[];
      setVariables(variables);
    };
    getVariable();
  }, []);
  return (
    <div className="p-2">
      <h1 className="font-bold">Variables</h1>
      <div className="mt-2">
        <Link
          to={"new"}
          type="button"
          className="bg-primary px-3 py-1 text-white"
        >
          Add
        </Link>
        <table className="mt-2 w-full table-auto">
          <thead>
            <tr>
              <th>Name</th>
              <th>Value Type</th>
              <th>Created at</th>
              <th>Updated at</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {variables.map((v) => (
              <tr key={v.id}>
                <td>
                  <Link to={`${v.id}/edit`} className="text-blue-500">
                    {v.name}
                  </Link>
                </td>
                <td>{v.valueType}</td>
                <td>{new Date(v.createdAt).toLocaleString()}</td>
                <td>
                  {v.updatedAt ? new Date(v.updatedAt).toLocaleString() : "-"}
                </td>
                <td className="inline-flex">
                  <button type="button" className="text-red-500">
                    Delete
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
