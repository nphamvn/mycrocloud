import { Link, useNavigate } from "react-router-dom";
import appData from "../../data/apps.json";
import App from "./App";
import { useEffect, useState } from "react";

export default function AppList() {
  const location = useNavigate();
  const [apps, setApps] = useState<App[]>([]);
  useEffect(() => {
    setTimeout(() => {
      const apps = appData as App[];
      setApps(apps);
    }, 100);
  }, []);
  return (
    <div className="mx-auto mt-2 max-w-4xl p-2">
      <div className="flex items-center">
        <h1 className="font-semibold">Your Apps</h1>
        <button
          type="button"
          className="group relative ms-auto flex items-center justify-center border border-transparent bg-cyan-700 p-0.5 text-center font-medium text-white focus:z-10 focus:outline-none focus:ring-2 focus:ring-cyan-300 enabled:hover:bg-cyan-800 dark:bg-cyan-600 dark:focus:ring-cyan-800 dark:enabled:hover:bg-cyan-700"
          onClick={() => location("/apps/new")}
        >
          <span className="flex items-center rounded-md px-3 py-1.5 text-sm transition-all duration-200">
            New
          </span>
        </button>
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
          className="block w-full border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
        />
      </form>
      <ul className="mt-3 divide-y">
        {apps.map((app) => {
          return (
            <li key={app.id}>
              <div className="mb-2">
                <Link to={`/apps/${app.id}`} className="text-slate-900">
                  {app.name}
                </Link>
                <p className="text-sm text-slate-600">{app.description}</p>
                <p className="text-sm text-slate-600">
                  Created: {new Date(app.createdAt).toDateString()}
                </p>
              </div>
            </li>
          );
        })}
      </ul>
    </div>
  );
}
