import Foundation
import Photos

// 写真へのアクセス権限リクエスト後のコールバック
public typealias OnRequestedCameraRollPermission = @convention(c) (Int32) -> Void

public class MocopiUILibrary: NSObject {
    
    // コールバック変数保持のためシングルトンにする
    public static let shared = MocopiUILibrary()
    private override init() {
        super.init()
    }
 
    // コールバック登録
    private var requestCameraRollCallback: OnRequestedCameraRollPermission?
    @objc static public func register(callback: @escaping OnRequestedCameraRollPermission) {
        MocopiUILibrary.shared.requestCameraRollCallback = callback
    }
    
    /// アプリケーションのディレクトリパスを取得
    /// - Returns: ディレクトリパス
    @objc public static func getApplicationDirectoryPath() -> String? {
        let applicationDirectoryPath = NSSearchPathForDirectoriesInDomains(.applicationSupportDirectory, .userDomainMask, true)[0]
        return applicationDirectoryPath
    }
    
    /// アプリケーションの設定画面を表示
    @objc public static func showApplicationSettings(){
        UIApplication.shared.open(URL(string: UIApplication.openSettingsURLString)!, options: [:], completionHandler: nil)
    }
    
    /// 指定した外部アプリが端末にインストールされているか確認する
    /// - Parameters:
    ///   - appScheme: 確認するアプリのスキーム
    /// - Returns: インストールされているか
    @objc public static func existsExternalApp(_ appScheme:String) -> Bool {
        let url = URL(string: appScheme)
        if let url{
            return UIApplication.shared.canOpenURL(url)
        }
        return false
    }
    
    /// VoiceOverがONか確認する
    /// - Returns: VoiceOverがONか
    @objc public static func isVoiceOverEnabled() -> Bool {
        return UIAccessibility.isVoiceOverRunning
    }
}
