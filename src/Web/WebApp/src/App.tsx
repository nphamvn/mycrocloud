import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import Home from "./components/Home";
import AppList from "./components/apps/AppList";
import AppCreate from "./components/apps/AppCreate";
import Header from "./components/Header";
import { Auth0Provider } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import AppIndex from "./components/apps/AppIndex";
import AppLogs from "./components/apps/AppLog";
import AppOverview from "./components/apps/AppOverview";
import RouteIndex from "./components/routes/RouteIndex";
import RouteLogs from "./components/routes/RouteLogs";
import RouteEdit from "./components/routes/RouteEdit";
import RouteCreate from "./components/routes/RouteCreate";
import SchemeList from "./components/authentications/SchemeList";
import AuthenticationsIndex from "./components/authentications/Authentication";
import CreateUpdateScheme from "./components/authentications/CreateUpdateScheme";
import AuthenticationSettings from "./components/authentications/Settings";
import AppVariables from "./components/apps/AppVariables";

function App() {
  return (
    <Auth0Provider
      domain={import.meta.env.VITE_AUTH0_DOMAIN}
      clientId={import.meta.env.VITE_AUTH0_CLIENTID}
      authorizationParams={{
        redirect_uri: window.location.origin,
        audience: import.meta.env.VITE_AUTH0_AUDIENCE,
      }}
    >
      <BrowserRouter>
        <Header />
        <div className="container mx-auto min-h-screen p-2">
          <Routes>
            <Route path="/" Component={Home} />
            <Route path="/apps" Component={AppList} />
            <Route path="/apps/new" Component={AppCreate} />
            <Route path="/apps/:appId" Component={AppIndex}>
              <Route index path="overview" Component={AppOverview} />
              <Route path="routes" Component={RouteIndex}>
                <Route path="new" Component={RouteCreate} />
                <Route path=":routeId" Component={RouteEdit} />
                <Route path=":routeId/logs" Component={RouteLogs} />
              </Route>
              <Route path="authentications" Component={AuthenticationsIndex}>
                <Route path="schemes" Component={SchemeList} />
                <Route path="schemes/new" Component={CreateUpdateScheme} />
                <Route
                  path="schemes/:schemeId"
                  Component={CreateUpdateScheme}
                />
                <Route path="settings" Component={AuthenticationSettings} />
              </Route>
              <Route path="logs" Component={AppLogs} />
              <Route path="variables" Component={AppVariables} />
            </Route>
          </Routes>
        </div>
        <ToastContainer />
      </BrowserRouter>
    </Auth0Provider>
  );
}
export default App;
