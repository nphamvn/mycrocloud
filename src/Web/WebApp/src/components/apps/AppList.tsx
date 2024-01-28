import { Link } from "react-router-dom";
import App from "./App";
import { useEffect, useState } from "react";
import { useAuth0 } from "@auth0/auth0-react";

export default function AppList() {
  const [searchTerm, setSearchTerm] = useState("");
  const [apps, setApps] = useState<App[]>([]);
  const { getAccessTokenSilently } = useAuth0();

  const getApps = async () => {
    const accessToken = await getAccessTokenSilently();
    const res = await fetch(`/api/apps?term=${searchTerm}`, {
      headers: {
        Authorization: `Bearer ${accessToken}`,
      },
    });
    const apps = (await res.json()) as App[];
    setApps(apps);
  };

  useEffect(() => {
    //TODO: debounce
    getApps();
  }, [searchTerm]);
  return (
    <div className="mx-auto mt-2 max-w-4xl p-2">
      <div className="flex items-center">
        <h1 className="font-semibold">Apps</h1>
        <Link
          to={"new"}
          className="group relative ms-auto flex items-center justify-center border border-transparent bg-primary p-0.5 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800 dark:bg-cyan-600 dark:focus:ring-cyan-800 dark:enabled:hover:bg-cyan-700"
        >
          <span className="flex items-center rounded-md px-3 py-1 text-sm transition-all duration-200">
            New
          </span>
        </Link>
      </div>
      <form className="mt-2">
        <label
          htmlFor="search-input"
          className="sr-only mb-2 text-sm font-medium text-gray-900 dark:text-white"
        >
          Search
        </label>
        <input
          type="text"
          id="search-input"
          onChange={(e) => {
            setSearchTerm(e.target.value);
          }}
          placeholder="Search..."
          className="block w-full border border-gray-300 bg-gray-50 p-1.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </form>
      <ul className="mt-3 divide-y">
        {apps.map((app) => {
          return (
            <li key={app.id}>
              <div className="mb-2">
                <h4>
                  <Link
                    to={`${app.id}/overview`}
                    className="font-semibold text-slate-900"
                  >
                    {app.name}
                  </Link>
                  <small
                    className={`ms-1 ${
                      app.status === "Active"
                        ? "text-green-500"
                        : "text-red-500"
                    }`}
                  >
                    {app.status}
                  </small>
                </h4>
                <p className="text-sm text-slate-600">{app.description}</p>
                <small className="text-sm text-slate-600">
                  Created: {new Date(app.createdAt).toDateString()}
                </small>
              </div>
            </li>
          );
        })}
      </ul>
    </div>
  );
}
