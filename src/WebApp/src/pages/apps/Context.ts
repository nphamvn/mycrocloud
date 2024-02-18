import { createContext } from "react";
import { IApp } from ".";

export const Context = createContext<IApp | undefined>(undefined);
