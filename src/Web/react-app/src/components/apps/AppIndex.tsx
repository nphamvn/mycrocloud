import { Link, Outlet, useLocation, useParams } from "react-router-dom";
import { AppContext } from "./AppContext";
import { useEffect, useState } from "react";
import App from "./App";
import { Breadcrumb } from "flowbite-react";

export default function AppIndex() {
  const appId = parseInt(useParams()["appId"]!.toString());
  const [app, setApp] = useState<App>();
  const { pathname } = useLocation();
  const path = pathname.split("/")[3];
  useEffect(() => {
    setTimeout(() => {
      const app = {
        id: appId,
        name: `App ${appId}`,
        createdAt: new Date().toString(),
        description: `App ${appId}`,
      };
      setApp(app);
    }, 100);
  }, []);
  useEffect(() => {
    const path = pathname.split("/")[3];
    if (app) {
      switch (path) {
        case "overview":
          document.title = app.name + " - Overview";
          break;
        case "routes":
          document.title = app.name + " - Routes";
          break;
        case "logs":
          document.title = app.name + " - Logs";
          break;
        default:
          document.title = app.name;
          break;
      }
    }
  }, [app, pathname]);
  if (!app) {
    return <h1>Loading...</h1>;
  }
  return (
    <AppContext.Provider value={app}>
      <div className="">
        <Breadcrumb className="p-1">
          <Breadcrumb.Item>
            <Link to="/">Home</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>
            <Link to="/apps">Apps</Link>
          </Breadcrumb.Item>
          <Breadcrumb.Item>{app.name}</Breadcrumb.Item>
        </Breadcrumb>
        <div className="flex min-h-screen flex-row border">
          <div className="flex w-32 flex-col space-y-0.5 border-r p-1">
            <Link
              to="overview"
              className={`${
                path === "overview" ? "text-cyan-700" : ""
              } text-sm`}
            >
              Overview
            </Link>
            <Link
              to="routes"
              className={`${path === "routes" ? "text-cyan-700" : ""} text-sm`}
            >
              Routes
            </Link>
            <Link
              to="logs"
              className={`${path === "logs" ? "text-cyan-700" : ""} text-sm`}
            >
              Logs
            </Link>
          </div>
          <div className="w-full">
            <Outlet />
          </div>
        </div>
      </div>
    </AppContext.Provider>
  );
}
