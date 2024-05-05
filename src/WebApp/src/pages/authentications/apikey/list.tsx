import { useContext, useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import IApiKey from "./IApiKey";
import { AppContext } from "../../apps";
import { useAuth0 } from "@auth0/auth0-react";
import { toast } from "react-toastify";

export default function List() {
  const { getAccessTokenSilently } = useAuth0();
  const navigate = useNavigate();
  const app = useContext(AppContext)!;
  const [keys, setKeys] = useState<IApiKey[]>([]);
  const [showingKeys, setShowingKeys] = useState<number[]>([]);

  useEffect(() => {
    const fetchKeys = async () => {
      const accessToken = await getAccessTokenSilently();
      const response = await fetch(`/api/apps/${app.id}/apikeys`, {
        headers: {
          authorization: `Bearer ${accessToken}`,
        },
      });
      const keys = await response.json();
      setKeys(keys);
    };
    fetchKeys();
  }, []);

  const handleDeleteClick = async (keyId: number) => {
    if (!window.confirm("Are you sure you want to delete this key?")) {
      return;
    }
    const accessToken = await getAccessTokenSilently();
    const response = await fetch(`/api/apps/${app.id}/apikeys/${keyId}`, {
      method: "DELETE",
      headers: { authorization: `Bearer ${accessToken}` },
    });

    if (response.ok) {
      setKeys(keys.filter((key) => key.id !== keyId));
    } else {
      toast("Failed to delete key");
    }
  };
  return (
    <div className="p-3">
      <h1 className="font-bold">API keys</h1>
      <button
        onClick={() => {
          navigate("new");
        }}
        className="mt-2 bg-primary px-2 py-1 text-white"
      >
        Create new key
      </button>
      <div>
        <table className="w-full">
          <thead>
            <tr>
              <th>Name</th>
              <th>Key</th>
              <th>Created At</th>
              <th>Updated At</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {keys.map((key) => (
              <tr key={key.id}>
                <td>{key.name}</td>
                <td className="flex space-x-1">
                  <span>
                    {showingKeys.includes(key.id)
                      ? key.key
                      : "****************"}
                  </span>
                  <button
                    onClick={() => {
                      if (showingKeys.includes(key.id)) {
                        setShowingKeys(showingKeys.filter((k) => k !== key.id));
                      } else {
                        setShowingKeys([...showingKeys, key.id]);
                      }
                    }}
                    className="text-primary hover:underline"
                  >
                    {showingKeys.includes(key.id) ? "Hide" : "Show"}
                  </button>
                  <button
                    onClick={() => {
                      navigator.clipboard.writeText(key.key);
                    }}
                    className="text-primary hover:underline"
                  >
                    Copy
                  </button>
                </td>
                <td>{new Date(key.createdAt).toDateString()}</td>
                <td>
                  {key.updatedAt ? new Date(key.updatedAt).toDateString() : "-"}
                </td>
                <td className="flex space-x-2">
                  <Link
                    to={`${key.id}/edit`}
                    className="text-primary hover:underline"
                  >
                    Edit
                  </Link>
                  <button
                    onClick={() => handleDeleteClick(key.id)}
                    className="text-red-500 hover:underline"
                  >
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
