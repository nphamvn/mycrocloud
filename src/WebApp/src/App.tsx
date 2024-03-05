import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import { Auth0Provider } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

import Header from "./components/Header";
import Home from "./components/Home";
import ProtectedPage from "./components/ProtectedPage";

import {
  AppList,
  AppOverview,
  AppCreate,
  AppLog,
  AppLayout,
} from "./pages/apps";

import { RouteIndex, RouteLogs, RouteCreate, RouteEdit } from "./pages/routes";

import {
  AuthenticationSchemeCreateUpdate,
  AuthenticationSchemeList,
  AuthenticationSchemeSettings,
} from "./pages/authentications";

import {
  VariableList,
  VariableCreateUpdate,
} from "./pages/storages/VariablesAndSecrets";

import {
  CreateUpdateTextStorage,
  LogonTextStorage,
  TextStorageList,
} from "./pages/storages/TextStorages";

import { DevPage, devRoutes } from "./components/devpages";
import AppInfo from "./pages/AppInfo";

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
            <Route
              path="/apps"
              element={<ProtectedPage children={<AppList />} />}
            />
            <Route
              path="/apps/new"
              element={<ProtectedPage children={<AppCreate />} />}
            />
            <Route
              path="/apps/:appId"
              element={<ProtectedPage children={<AppLayout />} />}
            >
              <Route index Component={AppOverview} />
              <Route path="routes" Component={RouteIndex}>
                <Route path="new" Component={RouteCreate} />
                <Route path=":routeId" Component={RouteEdit} />
                <Route path=":routeId/logs" Component={RouteLogs} />
              </Route>
              <Route path="authentications">
                <Route
                  index
                  path="schemes"
                  Component={AuthenticationSchemeList}
                />
                <Route
                  path="schemes/new"
                  Component={AuthenticationSchemeCreateUpdate}
                />
                <Route
                  path="schemes/:schemeId"
                  Component={AuthenticationSchemeCreateUpdate}
                />
                <Route
                  path="settings"
                  Component={AuthenticationSchemeSettings}
                />
              </Route>
              <Route path="logs" Component={AppLog} />
              <Route path="storages">
                <Route path="textstorages">
                  <Route index Component={TextStorageList} />
                  <Route path="new" Component={CreateUpdateTextStorage} />
                  <Route
                    path=":storageId"
                    Component={CreateUpdateTextStorage}
                  />
                  <Route path=":storageId/logon" Component={LogonTextStorage} />
                </Route>
                <Route path="variables">
                  <Route index Component={VariableList} />
                  <Route path="new" Component={VariableCreateUpdate} />
                  <Route
                    path=":variableId/edit"
                    Component={VariableCreateUpdate}
                  />
                </Route>
              </Route>
            </Route>
            <Route
              path="_appInfo"
              element={<ProtectedPage children={<AppInfo />} />}
            />
            {import.meta.env.DEV && (
              <Route path="_dev" Component={DevPage}>
                {devRoutes.map((r, i) => (
                  <Route key={i} path={r.path} Component={r.component} />
                ))}
              </Route>
            )}
          </Routes>
        </div>
        <ToastContainer />
      </BrowserRouter>
    </Auth0Provider>
  );
}
export default App;
