import { observer } from "mobx-react-lite";
import { AppHeader } from "../shared/components/Header";
import { useStore } from "../shared/stores";
import { NavigateFunction, useNavigate } from "react-router-dom";
import { useEffect, useState } from "react";
import { SignInForm } from "./SignInForm";
import { SignUpForm } from "./SignUpForm";

enum AuthMethod {
    SingIn,
    SignUp
}

export const AuthPage: React.FC = observer(() => {
    const [authMethod, setAuthMethod] = useState(AuthMethod.SingIn);

    const { authStore } = useStore();
    const navigate: NavigateFunction = useNavigate();

    useEffect(() => {
        if (authStore.isAuthenticated)
            navigate('/test');
    }, [authStore.isAuthenticated, navigate]);

    const goToSignUp = (): void => {
        setAuthMethod(AuthMethod.SignUp);
    }

    const goToSignIn = (): void => {
        setAuthMethod(AuthMethod.SingIn);
    }

    // TODO: Выделить компоненты форм, реализовать кастомные инпуты
    // TODO: На формах реализовать парсинг и вывод ошибок. Возможно в зависимости от провайдера аутентификации 
    // (возвращать с сервера или подумать над контрактом ошибок аутентификации, авторизации для всех провайдеров)
    return (
        <div style={{width: '100vw', height: '100vh', display: 'grid', gridTemplateColumns: 'auto 620px auto', gridTemplateRows: 'auto 1fr', minWidth: '620px'}}>
            <div style={{gridColumn: '1/4', border: '1px solid black'}}>
                <AppHeader/>
            </div>
            <div style={{gridColumn: '2/3', border: '1px solid black', display: 'flex', flexDirection: 'column', alignItems: 'center'}}>
                <div style={{height: '100px'}}/>
                {
                authMethod === AuthMethod.SingIn ?
                    <>
                    <SignInForm/>
                    <a onClick={goToSignUp}>Create free account</a>
                    </> :
                    <>
                    <SignUpForm/>
                    <a onClick={goToSignIn}>Sign in</a>
                    </>
                }
            </div>
        </div>
    );
});