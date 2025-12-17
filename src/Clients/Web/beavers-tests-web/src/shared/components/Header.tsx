import { useState } from "react";
import { UserDropdown } from "./UserDropdown";

export const AppHeader: React.FC = () => {
    const [isDropdownVisible, setIsDropdownVisible] = useState(false); 

    const handleUserClick = (): void => {
        setIsDropdownVisible(!isDropdownVisible);  
    };

    return (
        <>
        <div 
        style={{
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
            height: '48px',
            padding: '0px 10px'
        }}>
            <div>Logo</div>
            <div>Center</div>
            <div>
                <div>
                    <div style={{
                        width: '28px',
                        height: '28px',
                        cursor: 'pointer'
                    }}>
                        <div
                        onClick={handleUserClick} 
                        style={{
                            borderRadius: '25px',
                            background: 'gray',
                            width: '100%',
                            height: '100%'
                        }}>
                            { /* User account image */}
                        </div>
                    </div>
                </div>
            </div>
        </div>
        {isDropdownVisible && <UserDropdown/>}
        </>
    )
}