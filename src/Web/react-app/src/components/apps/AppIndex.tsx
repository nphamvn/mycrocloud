import { Link, Outlet, useParams } from "react-router-dom";
import { AppContext } from "./AppContext";
import { useEffect, useState } from "react";
import App from "./App";
import { Breadcrumb } from "flowbite-react";

export default function AppIndex() {
  const appId = parseInt(useParams()["appId"]!.toString());
  const [app, setApp] = useState<App>();
  useEffect(() => {
    setTimeout(() => {
      const app = {
        id: appId,
        name: `App ${appId}`,
        createdAt: new Date().toString(),
        description: `App ${appId}`,
      };
      setApp(app);
      document.title = app.name;
    }, 100);
  }, []);
  if (!app) {
    return <h1>Loading...</h1>;
  }
  return (
    <AppContext.Provider value={app}>
      <div className="min-h-full bg-slate-100">
        <Breadcrumb className="p-2">
          <Breadcrumb.Item>
            <Link to="/">Home</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>
            <Link to="/apps">Apps</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>{app.name}</Breadcrumb.Item>
        </Breadcrumb>
        <div className="flex flex-row p-2">
          <div className="flex w-32 flex-col space-y-0.5">
            <Link to="routes" color="primary" className="text-primary">
              Routes
            </Link>
            <Link to="logs">Logs</Link>
          </div>
          <div className="min-h-full w-full bg-slate-400">
            <Outlet />
          </div>
        </div>
      </div>
    </AppContext.Provider>
  );
}
