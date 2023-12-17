import { Link, useNavigate } from "react-router-dom";
import { Button } from "flowbite-react";
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
        <Button
          size="sm"
          className="ms-auto"
          onClick={() => location("/apps/new")}
        >
          New
        </Button>
      </div>
      <form className="mt-2">
        <label
          htmlFor="search-input"
          className="sr-only mb-2 text-sm font-medium text-gray-900 dark:text-white"
        >
          Search
        </label>
        <div className="relative">
          <input
            type="text"
            id="search-input"
            className="block w-full rounded-lg border border-gray-300 bg-gray-50 p-2.5 text-sm text-gray-900 focus:border-blue-500 focus:ring-blue-500 dark:border-gray-600 dark:bg-gray-700 dark:text-white dark:placeholder-gray-400 dark:focus:border-blue-500 dark:focus:ring-blue-500"
          />
        </div>
      </form>
      <ul className="mt-3">
        {apps.map((app) => {
          return (
            <li key={app.id}>
              <div className="mb-2">
                <Link to={`/apps/${app.id}`} className="text-slate-900">
                  {app.name}
                </Link>
                <p>{app.description}</p>
                <p className="text-gray text-sm font-light">
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
