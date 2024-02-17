import { BrowserRouter, Route, Routes } from "react-router-dom";
import "./App.css";
import Home from "./components/Home";
import AppList from "./pages/apps/AppList";
import AppCreate from "./pages/apps/AppCreate";
import Header from "./components/Header";
import { Auth0Provider } from "@auth0/auth0-react";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import AppLayout from "./pages/apps/AppLayout";
import AppLogs from "./pages/apps/AppLog";
import AppOverview from "./pages/apps/AppOverview";
import RouteIndex from "./pages/routes/RouteIndex";
import RouteLogs from "./pages/routes/RouteLogs";
import RouteEdit from "./pages/routes/RouteEdit";
import RouteCreate from "./pages/routes/RouteCreate";
import SchemeList from "./pages/authentications/SchemeList";
import AuthenticationsIndex from "./pages/authentications/Authentication";
import CreateUpdateScheme from "./pages/authentications/CreateUpdateScheme";
import AuthenticationSettings from "./pages/authentications/Settings";
import AppVariables from "./pages/storages/AppVariables";
import AddUpdateVariables from "./pages/storages/CreateUpdateVariables";
import {
  CreateUpdateTextStorage,
  LogonTextStorage,
  TextStorageList,
} from "./pages/storages/TextStorages";
import ProtectedPage from "./components/ProtectedPage";

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
                  <Route index Component={AppVariables} />
                  <Route path="new" Component={AddUpdateVariables} />
                  <Route
                    path=":variableId/edit"
                    Component={AddUpdateVariables}
                  />
                </Route>
              </Route>
            </Route>
          </Routes>
        </div>
        <ToastContainer />
      </BrowserRouter>
    </Auth0Provider>
  );
}
export default App;
