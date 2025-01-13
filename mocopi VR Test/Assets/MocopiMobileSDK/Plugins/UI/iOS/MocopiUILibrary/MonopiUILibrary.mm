#import <Foundation/Foundation.h>
#import <Photos/Photos.h>
#import <UnityFramework/UnityFramework-Swift.h>
#include "Unity/ObjCRuntime.h"
#include "UnityAppController.h"

typedef void (* capturedCameraImageCallback)();
typedef void (* cancelCapturedCameraImageCallback)();
typedef void (* requestCameraRollCallback)(int);

#ifdef __cplusplus
extern "C" {
#endif

// ステータスバー初期化フラグ
static bool isStatusBarInitialized = false;

// ステータスバーが非表示であるか
static bool isStatusBarHidden = false;

// prefersStatusBarHiddenの差し替え用メソッド
static bool prefersStatusBarHidden_Impl(id self_, SEL _cmd){
    return isStatusBarHidden;
}

// ステータスバーの表示切り替え
// 参考：https://qiita.com/youten_redo/items/94c233ac50871d8f49c7
// - param
//   - isHidden: 非表示であるか
static void SetStatusBarAppearance(bool isHidden) {
    UIViewController *viewController = GetAppController().rootViewController;
    if (!isStatusBarInitialized){
        class_replaceMethod([viewController class], @selector(prefersStatusBarHidden), (IMP)&prefersStatusBarHidden_Impl, UIViewController_prefersStatusBarHidden_Enc);
        isStatusBarInitialized = true;
    }

    isStatusBarHidden = isHidden;
    [viewController setNeedsStatusBarAppearanceUpdate];
}

// アプリケーションのディレクトリパスを取得
bool MocopiUILibrary_getApplicationDirectoryPath(char* outputBuffer, int bufferSize) {
    if (outputBuffer == nullptr) {
        return false;
    }
    NSString *applicationDirectoryPath = [ MocopiUILibrary getApplicationDirectoryPath ];
    if (applicationDirectoryPath != nil) {
        NSInteger applicationDirectoryPathLength = [ applicationDirectoryPath length ];
        if (applicationDirectoryPathLength < bufferSize) {
            const char *applicationDirectoryPathC = [ applicationDirectoryPath UTF8String ];
            strcpy(outputBuffer, applicationDirectoryPathC);
            return true;
        }
    }
    
    return false;
}

// 指定したキーで値を保存する関数
// - param
//   - dataType:  値を保存する際に用いるキー
//   - value: 保存したい値
// - return
//   - 保存時のステータスコードを返却する (0 以外は失敗)
int addItem(const char *dataType, const char *value){
    NSMutableDictionary* attributes = nil;
    NSMutableDictionary* query = [NSMutableDictionary dictionary];
    NSData* data = [[NSString stringWithCString:value encoding:NSUTF8StringEncoding] dataUsingEncoding:NSUTF8StringEncoding];
    
    [query setObject:(id)kSecClassGenericPassword forKey:(id)kSecClass];
    [query setObject:(id)[NSString stringWithCString:dataType encoding:NSUTF8StringEncoding] forKey:(id)kSecAttrAccount];
    OSStatus err = SecItemCopyMatching((CFDictionaryRef)query, NULL);
    if (err == noErr) {
        attributes = [NSMutableDictionary dictionary];
        [attributes setObject:data forKey:(id)kSecValueData];
        [attributes setObject:[NSDate date] forKey:(id)kSecAttrModificationDate];
        
        err = SecItemUpdate((CFDictionaryRef)query, (CFDictionaryRef)attributes);
        return (int)err;
    } else if (err == errSecItemNotFound) {
        attributes = [NSMutableDictionary dictionary];
        [attributes setObject:(id)kSecClassGenericPassword forKey:(id)kSecClass];
        [attributes setObject:(id)[NSString stringWithCString:dataType encoding:NSUTF8StringEncoding] forKey:(id)kSecAttrAccount];
        [attributes setObject:data forKey:(id)kSecValueData];
        [attributes setObject:[NSDate date] forKey:(id)kSecAttrCreationDate];
        [attributes setObject:[NSDate date] forKey:(id)kSecAttrModificationDate];
        err = SecItemAdd((CFDictionaryRef)attributes, NULL);
        return (int)err;
    } else {
        return (int)err;
    }
}
    
// 指定したキーで値を取得する関数
// - param
//   - dataType: 値を取得する際に用いるキー
// - return
//   - キーに紐づく値、存在しなければ空文字が返却される
char* getItem(const char *dataType) {
    NSMutableDictionary* query = [NSMutableDictionary dictionary];
    [query setObject:(id)kSecClassGenericPassword forKey:(id)kSecClass];
    [query setObject:(id)[NSString stringWithCString:dataType encoding:NSUTF8StringEncoding] forKey:(id)kSecAttrAccount];
    [query setObject:(id)kCFBooleanTrue forKey:(id)kSecReturnData];
    
    CFDataRef cfresult = NULL;
    OSStatus err = SecItemCopyMatching((CFDictionaryRef)query, (CFTypeRef*)&cfresult);
    if (err == noErr) {
        NSData* passwordData = (__bridge_transfer NSData *)cfresult;
        const char* value = [[[NSString alloc] initWithData:passwordData encoding:NSUTF8StringEncoding] UTF8String];
        char *str = strdup(value);
        return str;
    } else {
        return NULL;
    }
}
    
// 指定したキーで値を削除する関数
// - param
//   - dataType:  値を削除する際に用いるキー
// - return
//   - 保存時のステータスコードを返却する (0 以外は失敗)
int deleteItem(const char *dataType){
    NSMutableDictionary* query = [NSMutableDictionary dictionary];
    [query setObject:(id)kSecClassGenericPassword forKey:(id)kSecClass];
    [query setObject:(id)[NSString stringWithCString:dataType encoding:NSUTF8StringEncoding] forKey:(id)kSecAttrAccount];
    
    OSStatus err = SecItemDelete((CFDictionaryRef)query);
    
    if (err == noErr) {
        return 0;
    } else {
        return (int)err;
    }
}

// アプリケーションの設定画面を表示
void MocopiUILibrary_showApplicationSettings() {
    return [MocopiUILibrary showApplicationSettings];
}

// ステータスバーを表示
void MocopiUILibrary_showStatusBar() {
    SetStatusBarAppearance(false);
}

// ステータスバーを非表示
void MocopiUILibrary_hideStatusBar() {
    SetStatusBarAppearance(true);
}

// ディスプレイ輝度の設定
void MocopiUILibrary_setBrightness(float val) {
    [UIScreen mainScreen].brightness = val; 
}

// ディスプレイ輝度の取得
float MocopiUILibrary_getBrightness() {
    return [UIScreen mainScreen].brightness;
}

// 指定した外部アプリが端末にインストールされているか確認する
// - param
//   - appScheme:  確認するアプリのスキーム
// - return
//   - インストールされているか
bool MocopiUILibrary_existsExternalApp(char *appScheme) {
    NSString* appSchemeString = [NSString stringWithCString:appScheme encoding:NSUTF8StringEncoding];
    return [MocopiUILibrary existsExternalApp:appSchemeString];
}

// VoiceOverがONか確認する
// - return
//   - VoiceOverがONか
bool MocopiUILibrary_isVoiceOverEnabled() {
    return [MocopiUILibrary isVoiceOverEnabled];
}

#ifdef __cplusplus
}
#endif
