export interface UserRegister {
    userName: string;
    password?: string; 
    confirmPassword?: string;
}

export interface UserLogin {
    userName: string;
    password?: string;
}

export interface User {
    id: string;
    userName: string;
}

export interface AuthResult {
    succeeded: boolean;
    errors: string[];
    token?: string;
    expiration?: Date;
    user?: User;
}