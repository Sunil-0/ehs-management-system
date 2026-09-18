export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  userId: number;
  name: string;
  role: string; // "Employee" | "EHSManager" | "Investigator" | "Manager"
}