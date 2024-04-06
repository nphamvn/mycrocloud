export default interface IForm {
  id: number;
  name: string;
  description: string;
  createdAt: string;
  fields: IFormField[];
}

export interface IFormField {
  id: string;
  name: string;
  type: "TextInput" | "NumberInput";
  details: IFormDetails;
}

export interface IFormDetails {
  textInput?: ITextInput;
  numberInput?: INumberInput;
}

export interface ITextInput {
  minLength?: number;
  maxLength?: number;
}

export interface INumberInput {
  min?: number;
  max?: number;
}
