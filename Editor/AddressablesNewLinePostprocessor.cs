#if UNITY_EDITOR_WIN

using System.IO;
using System.Linq;
using UnityEditor;

namespace Kogane.Internal
{
	/// <summary>
	/// Addressable のアセットがインポートされた時に改行文字を Win に統一するエディタ拡張
	/// </summary>
	internal sealed class AddressablesNewLinePostprocessor : AssetPostprocessor
	{
		//================================================================================
		// 定数
		//================================================================================
		private const string WINDOWS = "\r\n";
		private const string UNIX    = "\n";
		private const string MAC     = "\r";

		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// アセットがインポートされた時などに呼び出されます
		/// </summary>
		private static void OnPostprocessAllAssets
		(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths
		)
		{
			var list = importedAssets
					.Where
					(
						x => x.StartsWith( "Assets/AddressableAssetsData/AssetGroups/Schemas/" ) ||
						     x.EndsWith( "AddressableAssetSettings.asset" )
					)
					.ToArray()
				;

			if ( list.Length <= 0 ) return;

			for ( var i = 0; i < list.Length; i++ )
			{
				var path    = list[ i ];
				var oldText = File.ReadAllText( path );
				var newText = oldText;

				// 一度 Win -> Unix に変換しないと改行コードの変換が成功しなかった
				newText = newText.Replace( WINDOWS, UNIX );

				newText = newText.Replace( MAC, UNIX );
				newText = newText.Replace( UNIX, WINDOWS );

				// 処理時間削減のため、
				// 改行コードの変換が不要であればファイルの書き込みは行わない
				if ( newText == oldText ) continue;

				File.WriteAllText( path, newText );
			}
		}
	}
}

#endif